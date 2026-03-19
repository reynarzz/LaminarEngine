using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SerializableGuid
    {
        [SerializedField] private ulong _a;
        [SerializedField] private ulong _b;

        public Guid Guid
        {
            get
            {
                Span<byte> bytes = stackalloc byte[16];
                BitConverter.TryWriteBytes(bytes.Slice(0, 8), _a);
                BitConverter.TryWriteBytes(bytes.Slice(8, 8), _b);
                return new Guid(bytes);
            }
            set
            {
                Span<byte> bytes = stackalloc byte[16];
                value.TryWriteBytes(bytes);
                _a = BitConverter.ToUInt64(bytes.Slice(0, 8));
                _b = BitConverter.ToUInt64(bytes.Slice(8, 8));
            }
        }

        public static implicit operator SerializableGuid(Guid guid)
        {
            return new SerializableGuid() { Guid = guid };
        }

        public static implicit operator Guid(SerializableGuid guid)
        {
            return guid.Guid;
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
        public override string ToString()
        {
            return Guid.ToString();
        }
    }
}
