using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Engine.Serialization
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializedField] private ulong _a;
        [SerializedField] private ulong _b;

        public SerializableGuid(ulong a, ulong b)
        {
            _a = a;
            _b = b;
        }

        public Guid Guid
        {
            get
            {
                Span<byte> bytes = stackalloc byte[16];
                BinaryPrimitives.WriteUInt64LittleEndian(bytes[..8], _a);
                BinaryPrimitives.WriteUInt64LittleEndian(bytes[8..], _b);
                return new Guid(bytes);
            }
        }

        public static implicit operator SerializableGuid(Guid guid)
        {
            Span<byte> bytes = stackalloc byte[16];
            guid.TryWriteBytes(bytes);
            return new SerializableGuid(BinaryPrimitives.ReadUInt64LittleEndian(bytes[..8]),
                                        BinaryPrimitives.ReadUInt64LittleEndian(bytes[8..]));
        }

        public static implicit operator Guid(SerializableGuid guid)
        {
            return guid.Guid;
        }

        public bool Equals(SerializableGuid other)
        {
            return _a == other._a && _b == other._b;
        }

        public override bool Equals(object? obj)
        {
            return obj is SerializableGuid other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_a, _b);
        }

        public override string ToString()
        {
            return Guid.ToString();
        }

        public static bool operator ==(SerializableGuid a, SerializableGuid b)
        {
            return a._a == b._a && a._b == b._b;
        }

        public static bool operator !=(SerializableGuid a, SerializableGuid b)
        {
            return a._a != b._a || a._b != b._b;
        }
        public static bool operator ==(Guid a, SerializableGuid b)
        {
            return a == b.Guid;
        }

        public static bool operator !=(Guid a, SerializableGuid b)
        {
            return a != b.Guid;
        }
        public static bool operator ==(SerializableGuid b, Guid a)
        {
            return a == b.Guid;
        }

        public static bool operator !=(SerializableGuid b, Guid a)
        {
            return a != b.Guid;
        }
    }
}