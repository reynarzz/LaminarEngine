using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace Engine.Serialization
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RefId : IEquatable<RefId>, IComparable<RefId>
    {
        [SerializedField] private ulong _high;
        [SerializedField] private ulong _low;

        public static readonly RefId Empty = default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RefId(ulong high, ulong low)
        {
            _high = high;
            _low = low;
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _high == 0 && _low == 0;
        }

        public long TimestampMs
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (long)(_high >> 16);
        }

        public DateTimeOffset CreatedAt
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => DateTimeOffset.FromUnixTimeMilliseconds(TimestampMs);
        }

        public ushort NodeId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ushort)(_low >> 48);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RefId other)
        {
            return _high == other._high && _low == other._low;
        }

        public override bool Equals(object? obj)
        {
            return obj is RefId other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(_high, _low);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(RefId other)
        {
            int c = _high.CompareTo(other._high);
            return c != 0 ? c : _low.CompareTo(other._low);
        }

        public static bool operator ==(RefId a, RefId b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(RefId a, RefId b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return $"{_high:x16}-{_low:x16}";
        }

        public static RefId Parse(string s)
        {
            return Parse(s.AsSpan());
        }

        public static RefId Parse(ReadOnlySpan<char> s)
        {
            if (!TryParse(s, out RefId id))
            {
                throw new FormatException($"Input is not a valid RefId. " +
                                          $"Expected 'xxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxx' (16 hex, dash, 16 hex). " +
                                          $"Got: '{s.ToString()}'");
            }
            return id;
        }

        public static bool TryParse(string s, out RefId id)
        {
            return TryParse(s.AsSpan(), out id);
        }

        public static bool TryParse(ReadOnlySpan<char> s, out RefId id)
        {
            if (s.Length != 33 || s[16] != '-')
            {
                id = Empty;
                return false;
            }

            if (!TryParseHex16(s[..16], out ulong high) ||
                !TryParseHex16(s[17..], out ulong low))
            {
                id = Empty;
                return false;
            }

            id = new RefId(high, low);
            return true;
        }

        private static bool TryParseHex16(ReadOnlySpan<char> s, out ulong value)
        {
            value = 0;
            if (s.Length != 16)
            {
                return false;
            }

            for (int i = 0; i < 16; i++)
            {
                char c = s[i];
                uint nibble;

                if (c >= '0' && c <= '9') nibble = (uint)(c - '0');
                else if (c >= 'a' && c <= 'f') nibble = (uint)(c - 'a' + 10);
                else if (c >= 'A' && c <= 'F') nibble = (uint)(c - 'A' + 10);
                else return false;

                value = (value << 4) | nibble;
            }

            return true;
        }

        internal static RefId Create(ulong high, ulong low)
        {
            return new RefId(high, low);
        }

        public static RefId New()
        {
            return RefIdGenerator.New();
        }
    }

    internal static class NodeConfig
    {
        private static long _packed = 0;

        public static ushort NodeId
        {
            get
            {
                long packed = Volatile.Read(ref _packed);
                if ((packed & 0x1_0000L) == 0)
                {
                    InitializeFromMachineName();
                    packed = Volatile.Read(ref _packed);
                }
                return (ushort)(packed & 0xFFFF);
            }
        }

        public static void Initialize(ushort nodeId)
        {
            Volatile.Write(ref _packed, 0x1_0000L | nodeId);
        }

        public static void InitializeFromMachineName()
        {
            string name = Environment.MachineName;
            uint hash = 2166136261u;
            foreach (char c in name)
            {
                hash ^= (byte)c;
                hash *= 16777619u;
            }
            Initialize((ushort)(hash & 0xFFFF));
        }

        public static void InitializeFromEnvironment(string variableName = "ENGINE_NODE_ID")
        {
            string? value = Environment.GetEnvironmentVariable(variableName);
            if (value != null && ushort.TryParse(value, out ushort nodeId))
            {
                Initialize(nodeId);
            }
            else
            {
                InitializeFromMachineName();
            }
        }
    }

    internal static class RefIdGenerator
    {
        private static long _state = 0;

        private static volatile bool _deterministicMode = false;
        private static ulong _deterministicSeed = 1;

        [ThreadStatic] private static byte[]? _csBuffer;
        [ThreadStatic] private static int _csBufferPos;

        [ThreadStatic] private static ulong _smState;

        private const int CsBufferSize = 4096;

        public static void SetDeterministic(ulong seed)
        {
            _deterministicSeed = seed != 0 ? seed : 1;
            _deterministicMode = true;
            _smState = 0;
        }

        public static void SetNormal()
        {
            _deterministicMode = false;
            _smState = 0;
        }

        public static RefId New()
        {
            ulong high = NextHigh();
            ulong low = NextLow();
            return RefId.Create(high, low);
        }

        private static ulong NextHigh()
        {
            long ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            while (true)
            {
                long old = Volatile.Read(ref _state);
                long oldMs = old >> 16;
                int oldSeq = (int)(old & 0xFFFF);

                long newMs = ms > oldMs ? ms : oldMs;
                int newSeq = newMs == oldMs ? oldSeq + 1 : 0;

                if (newSeq > 0xFFFF)
                {
                    SpinWait sw = default;
                    do
                    {
                        sw.SpinOnce();
                        ms = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    }
                    while (ms <= oldMs);

                    continue;
                }

                long next = (newMs << 16) | (ushort)newSeq;

                if (Interlocked.CompareExchange(ref _state, next, old) == old)
                    return (ulong)next;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong NextLow()
        {
            ulong rand = _deterministicMode ? NextSplitMix64() : NextCsprng64();

            ulong node = NodeConfig.NodeId;
            return (node << 48) | (rand & 0x0000FFFFFFFFFFFF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong NextSplitMix64()
        {
            if (_smState == 0)
            {
                ulong root = _deterministicSeed;
                ulong thread = (ulong)Environment.CurrentManagedThreadId;

                ulong z = (root ^ (thread * 0x9E3779B97F4A7C15UL)) | 1UL;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                _smState = z ^ (z >> 31);
            }

            ulong s = _smState += 0x9E3779B97F4A7C15UL;
            s = (s ^ (s >> 30)) * 0xBF58476D1CE4E5B9UL;
            s = (s ^ (s >> 27)) * 0x94D049BB133111EBUL;
            return s ^ (s >> 31);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong NextCsprng64()
        {
            if (_csBuffer == null || _csBufferPos > CsBufferSize - 8)
            {
                _csBuffer ??= new byte[CsBufferSize];
                RandomNumberGenerator.Fill(_csBuffer);
                _csBufferPos = 0;
            }

            ulong value = BinaryPrimitives.ReadUInt64LittleEndian(
                _csBuffer.AsSpan(_csBufferPos, 8));
            _csBufferPos += 8;
            return value;
        }
    }
}