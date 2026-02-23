using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    internal class EngineFileUtils
    {
        internal static unsafe Guid ReadGuidNoAlloc(BinaryReader reader)
        {
            Guid guid;
            reader.Read(new Span<byte>(&guid, sizeof(Guid)));
            return guid;
        }

        internal static string ReadString(BinaryReader reader)
        {
            var totalBytes = reader.ReadInt32();

            if (totalBytes <= 0)
            {
                return string.Empty;
            }

            if (totalBytes <= 1024) // a kb
            {
                Span<byte> buffer = stackalloc byte[totalBytes];
                reader.BaseStream.ReadExactly(buffer);
                return Encoding.UTF8.GetString(buffer);
            }
            else
            {
                var buffer = ArrayPool<byte>.Shared.Rent(totalBytes);
                try
                {
                    var span = buffer.AsSpan(0, totalBytes);
                    reader.BaseStream.ReadExactly(span);
                    return Encoding.UTF8.GetString(span);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }

        internal static bool ByteToBool(byte value)
        {
            return value != 0;
        }
        internal static bool ReadBool(BinaryReader reader)
        {
            return ByteToBool(reader.ReadByte());
        }

        internal static unsafe T ReadStructNoAlloc<T>(BinaryReader reader) where T : unmanaged
        {
            Span<byte> buff = stackalloc byte[sizeof(T)];
            reader.BaseStream.ReadExactly(buff);
            return MemoryMarshal.Read<T>(buff);
        }

        internal static T[] ReadArray<T>(BinaryReader reader, int count) where T : unmanaged
        {
            var values = new T[count];
            Span<byte> bytes = MemoryMarshal.AsBytes(values.AsSpan());
            reader.BaseStream.ReadExactly(bytes);
            return values;
        }
    }
}
