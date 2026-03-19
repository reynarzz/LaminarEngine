using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;

namespace Engine.Serialization
{
    // Tier 1 - Runtime handle (hot path)
    // 64-bit generational index. Zero allocation. No syscalls. ~1 ns.
    // Layout: [ type:8 | index:24 | generation:32 ]
    // Never persisted. Stale handles detected in O(1) by comparing the generation field.
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct EntityId : IEquatable<EntityId>
    {
        private readonly ulong _value;

        public static readonly EntityId Invalid = default;

        private const ulong TypeMask = 0xFF00_0000_0000_0000UL;
        private const ulong IndexMask = 0x00FF_FFFF_0000_0000UL;
        private const ulong GenerationMask = 0x0000_0000_FFFF_FFFFUL;
        private const int TypeShift = 56;
        private const int IndexShift = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private EntityId(ulong value) => _value = value;

        /// <summary>Entity category (mesh, light, audio, physics, …)</summary>
        public byte Type
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (byte)((_value & TypeMask) >> TypeShift);
        }

        /// <summary>Slot index in the entity pool. Reused after destruction.</summary>
        public uint Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (uint)((_value & IndexMask) >> IndexShift);
        }

        /// <summary>
        /// Bumped every time a slot is recycled.
        /// Stale handles (old generation) are detected by a single integer compare.
        /// </summary>
        public uint Generation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (uint)(_value & GenerationMask);
        }

        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _value != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EntityId Create(byte type, uint index, uint generation)
        {
            ulong v = ((ulong)type << TypeShift)
                    | ((ulong)index << IndexShift)
                    | (ulong)generation;
            return new EntityId(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EntityId BumpGeneration() => Create(Type, Index, Generation + 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(EntityId other) => _value == other._value;

        public override bool Equals(object? obj) => obj is EntityId e && Equals(e);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(EntityId a, EntityId b) => a._value == b._value;
        public static bool operator !=(EntityId a, EntityId b) => a._value != b._value;

        public override string ToString() => $"EID[t={Type} i={Index} g={Generation}]";
    }


    // EntityPool - generational free-list for Tier 1 handles.
    // Thread-safe via interlocked CAS on the free-list head. Alloc and Free are both O(1).
    public sealed class EntityPool
    {
        private const int MaxEntities = 1 << 24; // 16M slots

        // _generation[i]: top bit = live flag, lower 31 bits = generation counter.
        private readonly uint[] _generation;
        // _next[i]: free-list chain. uint.MaxValue = end of list.
        private readonly uint[] _next;

        private int _freeHead; // CAS-protected

        public EntityPool()
        {
            _generation = new uint[MaxEntities];
            _next = new uint[MaxEntities];

            for (int i = 0; i < MaxEntities - 1; i++)
            {
                _next[i] = (uint)(i + 1);
            }
            _next[MaxEntities - 1] = uint.MaxValue;

            _freeHead = 0;
        }

        /// <summary>Allocate a slot. Returns <see cref="EntityId.Invalid"/> if the pool is full.</summary>
        public EntityId Allocate(byte type)
        {
            while (true)
            {
                int head = Volatile.Read(ref _freeHead);
                if (head == -1) return EntityId.Invalid;

                uint next = _next[head];
                int nextHead = next == uint.MaxValue ? -1 : (int)next;

                if (Interlocked.CompareExchange(ref _freeHead, nextHead, head) != head)
                {
                    continue;
                }

                uint gen = _generation[head] & 0x7FFF_FFFF; // strip live bit
                _generation[head] = gen | 0x8000_0000;      // set live bit
                return EntityId.Create(type, (uint)head, gen);
            }
        }

        /// <summary>
        /// Free a slot. Bumps generation so stale EntityIds are immediately detectable.
        /// Double-free safe — silently ignores stale or already-freed handles.
        /// </summary>
        public void Free(EntityId id)
        {
            uint index = id.Index;
            uint current = Volatile.Read(ref _generation[index]);

            if ((current & 0x7FFF_FFFF) != id.Generation) return; // stale
            if ((current & 0x8000_0000) == 0) return;              // already free

            uint bumped = (id.Generation + 1) & 0x7FFF_FFFF; // wrap at 31 bits
            _generation[index] = bumped;                      // clear live bit, bump gen

            while (true)
            {
                int head = Volatile.Read(ref _freeHead);
                _next[index] = head == -1 ? uint.MaxValue : (uint)head;
                if (Interlocked.CompareExchange(ref _freeHead, (int)index, head) == head)
                {
                    break;
                }
            }
        }

        /// <summary>Returns true if the handle refers to a currently-live entity.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAlive(EntityId id)
        {
            uint current = Volatile.Read(ref _generation[id.Index]);
            return (current & 0x8000_0000) != 0
                && (current & 0x7FFF_FFFF) == id.Generation;
        }
    }


    // Tier 2 - Persistent asset / content ID (cold path, editor & build)
    // 128-bit. Monotonic. Sortable. Stable across machines and sessions.
    //
    // Layout:
    //   _high [ 48 bits: engine frame counter | 16 bits: sequence ]
    //   _low  [ 16 bits: node ID              | 48 bits: random   ]
    //
    // String format: "xxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxx" (16 hex + '-' + 16 hex)
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct RefId : IEquatable<RefId>, IComparable<RefId>
    {
        private readonly ulong _high;
        private readonly ulong _low;

        public static readonly RefId Empty = default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private RefId(ulong high, ulong low) { _high = high; _low = low; }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _high == 0 && _low == 0;
        }

        /// <summary>Engine frame at which this ID was minted.</summary>
        public long Frame
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (long)(_high >> 16);
        }

        /// <summary>Per-frame sequence number (lower 16 bits of _high).</summary>
        public ushort Sequence
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ushort)(_high & 0xFFFF);
        }

        /// <summary>Node that generated this ID (upper 16 bits of _low). Never 0.</summary>
        public ushort NodeId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (ushort)(_low >> 48);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RefId other) => _high == other._high && _low == other._low;

        public override bool Equals(object? obj) => obj is RefId other && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(_high, _low);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(RefId other)
        {
            int c = _high.CompareTo(other._high);
            return c != 0 ? c : _low.CompareTo(other._low);
        }

        public static bool operator ==(RefId a, RefId b) => a.Equals(b);
        public static bool operator !=(RefId a, RefId b) => !a.Equals(b);

        public override string ToString() => $"{_high:x16}-{_low:x16}";

        public static RefId Parse(string s) => Parse(s.AsSpan());

        public static RefId Parse(ReadOnlySpan<char> s)
        {
            if (!TryParse(s, out RefId id))
            {
                throw new FormatException(
                    $"Expected 'xxxxxxxxxxxxxxxx-xxxxxxxxxxxxxxxx'. Got: '{s.ToString()}'");
            }
            return id;
        }

        public static bool TryParse(string s, out RefId id) => TryParse(s.AsSpan(), out id);

        public static bool TryParse(ReadOnlySpan<char> s, out RefId id)
        {
            if (s.Length != 33 || s[16] != '-') { id = Empty; return false; }
            if (!TryParseHex16(s[..16], out ulong high) ||
                !TryParseHex16(s[17..], out ulong low)) { id = Empty; return false; }
            id = new RefId(high, low);
            return true;
        }

        private static bool TryParseHex16(ReadOnlySpan<char> s, out ulong value)
        {
            value = 0;
            if (s.Length != 16) return false;
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

        /// <summary>Mint a new unique persistent ID.</summary>
        public static RefId New() => RefIdGenerator.New();

        internal static RefId Create(ulong high, ulong low) => new RefId(high, low);
    }


    // NodeConfig - configure once at engine startup.
    // Each machine or project that shares an asset database must have a unique NodeId.
    // NodeId 0 is reserved as "uninitialized / invalid".
    //
    // Options:
    //   NodeConfig.Initialize(42);               // explicit (recommended)
    //   NodeConfig.InitializeFromEnvironment();  // reads ENGINE_NODE_ID env var
    //   NodeConfig.InitializeFromMachineName();  // zero-config fallback
    //
    // If Initialize is never called the getter auto-seeds from machine name.
    // All three paths produce NodeId >= 1.
    public static class NodeConfig
    {
        // _packed layout: bit 16 = initialized flag, bits 0-15 = node ID.
        private static int _packed = 0;

        /// <summary>Node ID for this machine/project (1-65535). Never 0.</summary>
        public static ushort NodeId
        {
            get
            {
                int packed = Volatile.Read(ref _packed);
                if ((packed & 0x1_0000) == 0)
                {
                    InitializeFromMachineName();
                    packed = Volatile.Read(ref _packed);
                }
                return (ushort)(packed & 0xFFFF);
            }
        }

        /// <summary>
        /// Explicitly set the node ID. Call once at engine startup before any
        /// RefId is generated. NodeId 0 is remapped to 1 to preserve the Empty sentinel.
        /// </summary>
        public static void Initialize(ushort nodeId)
        {
            if (nodeId == 0) nodeId = 1;
            Volatile.Write(ref _packed, 0x1_0000 | nodeId);
        }

        /// <summary>
        /// Derives node ID from a stable FNV-1a hash of the machine name.
        /// Avoids string.GetHashCode() which is not stable across .NET versions or process restarts.
        /// </summary>
        public static void InitializeFromMachineName()
        {
            string name = Environment.MachineName;
            uint hash = 2166136261u; // FNV-1a offset basis
            foreach (char c in name)
            {
                hash ^= (byte)c;
                hash *= 16777619u; // FNV prime
            }
            ushort nodeId = (ushort)((hash & 0x7FFF) | 1); // LSB forced = always odd, never 0
            Initialize(nodeId);
        }

        /// <summary>Reads node ID from an environment variable; falls back to machine name.</summary>
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


    // EngineFrame - static frame counter for Tier 2 ID minting.
    // Call EngineFrame.Advance() exactly once per game loop tick from the main thread.
    public static class EngineFrame
    {
        private static long _frame = 0;

        /// <summary>Current engine frame. Monotonically increasing within a session.</summary>
        public static long Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Volatile.Read(ref _frame);
        }

        /// <summary>Call exactly once per game loop tick from the main thread.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Advance() => Interlocked.Increment(ref _frame);

        /// <summary>Reset for testing or level reload. Not safe to call during ID generation.</summary>
        public static void Reset(long value = 0) => Volatile.Write(ref _frame, value);
    }


    // RefIdGenerator - Tier 2 ID minting. Internal, thread-safe, lock-free.
    //
    // Normal mode:       Per-thread Xorshift64 seeded from CSPRNG once per thread lifetime.
    //                    No syscall per ID. ~3 ns per call. Period 2^64-1. Passes BigCrush.
    // Deterministic mode: Per-thread SplitMix64 seeded from a fixed root + thread ID.
    //                    Reproducible across runs given the same seed and thread count.
    internal static class RefIdGenerator
    {
        private static long _state = 0;

        private static volatile bool _deterministicMode = false;
        private static volatile uint _deterministicSeedHigh = 0;
        private static volatile uint _deterministicSeedLow = 1;

        [ThreadStatic] private static ulong _xsState;
        [ThreadStatic] private static ulong _smState;
        [ThreadStatic] private static bool _smSeeded;

        /// <summary>
        /// Switch to deterministic mode. All threads will produce reproducible
        /// IDs given the same seed and call order. Useful for content pipeline
        /// tests and snapshot testing of asset manifests. seed 0 is remapped to 1.
        /// </summary>
        public static void SetDeterministic(ulong seed)
        {
            ulong s = seed != 0 ? seed : 1;
            Volatile.Write(ref _deterministicSeedHigh, (uint)(s >> 32));
            Volatile.Write(ref _deterministicSeedLow, (uint)(s & 0xFFFFFFFF));
            _deterministicMode = true;
            _smState = 0;
            _smSeeded = false;
        }

        /// <summary>Switch back to normal (CSPRNG-seeded Xorshift64) mode.</summary>
        public static void SetNormal()
        {
            _deterministicMode = false;
            _xsState = 0;
            _smState = 0;
            _smSeeded = false;
        }

        public static RefId New()
        {
            ulong high = NextHigh();
            ulong low = NextLow();
            return RefId.Create(high, low);
        }

        private static ulong NextHigh()
        {
            long frame = EngineFrame.Current;

            while (true)
            {
                long old = Volatile.Read(ref _state);
                long oldF = old >> 16;
                int oldSeq = (int)(old & 0xFFFF);

                long newF = frame > oldF ? frame : oldF;
                int newSeq = newF == oldF ? oldSeq + 1 : 0;

                if (newSeq > 0xFFFF)
                {
                    // Over 65,535 asset IDs minted in a single frame. Spin until next frame.
                    SpinWait sw = default;
                    do
                    {
                        sw.SpinOnce();
                        frame = EngineFrame.Current;
                    }
                    while (frame <= oldF);
                    continue;
                }

                long next = (newF << 16) | (ushort)newSeq;
                if (Interlocked.CompareExchange(ref _state, next, old) == old)
                {
                    return (ulong)next;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong NextLow()
        {
            ulong rand = _deterministicMode ? NextSplitMix64() : NextXorshift64();
            ulong node = NodeConfig.NodeId; // always >= 1
            return (node << 48) | (rand & 0x0000_FFFF_FFFF_FFFFUL);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong NextXorshift64()
        {
            if (_xsState == 0)
            {
                _xsState = CryptoSeed();
            }

            ulong x = _xsState;
            x ^= x << 13;
            x ^= x >> 7;
            x ^= x << 17;
            return _xsState = x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong NextSplitMix64()
        {
            if (!_smSeeded)
            {
                ulong root = ((ulong)Volatile.Read(ref _deterministicSeedHigh) << 32)
                              | Volatile.Read(ref _deterministicSeedLow);
                ulong thread = (ulong)Environment.CurrentManagedThreadId;
                ulong z = (root ^ (thread * 0x9E3779B97F4A7C15UL)) | 1UL;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                _smState = z ^ (z >> 31);
                _smSeeded = true;
            }

            ulong s = _smState += 0x9E3779B97F4A7C15UL;
            s = (s ^ (s >> 30)) * 0xBF58476D1CE4E5B9UL;
            s = (s ^ (s >> 27)) * 0x94D049BB133111EBUL;
            return s ^ (s >> 31);
        }

        private static ulong CryptoSeed()
        {
            Span<byte> buf = stackalloc byte[8];
            RandomNumberGenerator.Fill(buf);
            return BinaryPrimitives.ReadUInt64LittleEndian(buf) | 1UL; // Xorshift64 must never hold state 0
        }
    }
}