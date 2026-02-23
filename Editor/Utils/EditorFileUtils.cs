using Engine;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal static class EditorFileUtils
    {
        public static bool IsNativeDll(string dllPath)
        {
            if (!File.Exists(dllPath))
            {
                Debug.Error($"Dll not found at path: {dllPath}");
                return false;
            }

            try
            {
                AssemblyName.GetAssemblyName(dllPath);
                return false;
            }
            catch (BadImageFormatException)
            {
                return true;
            }
        }

        private const byte ObfuscationKey = 0x77; // Secret engine key

        public static void WriteObfuscatedString(this BinaryWriter writer, string value)
        {
            if (value == null)
            {
                writer.Write(-1); // Represent null as -1 length
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= ObfuscationKey;
            }

            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public static string ReadObfuscatedString(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            if (length == -1) return null;

            byte[] bytes = reader.ReadBytes(length);

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= ObfuscationKey;
            }

            return Encoding.UTF8.GetString(bytes);
        }

        internal static unsafe void WriteGuidNoAlloc(BinaryWriter writer, in Guid id)
        {
            Span<byte> bytes = stackalloc byte[sizeof(Guid)];
            id.TryWriteBytes(bytes);
            writer.Write(bytes);
        }

        internal static void WriteSpan<T>(BinaryWriter writer, T[] values) where T : unmanaged
        {
            ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes(values.AsSpan());
            writer.Write(bytes);
        }
        internal static void WriteStruct<T>(BinaryWriter writer, in T value) where T : unmanaged
        {
            Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write(buffer, in value);
            writer.Write(buffer);
        }
        internal static void WriteString(BinaryWriter writer, string str, int chunkSize = 8192)
        {
            if (string.IsNullOrEmpty(str))
            {
                writer.Write(0);
                return;
            }

            int totalBytes = Encoding.UTF8.GetByteCount(str);
            writer.Write(totalBytes);

            if (totalBytes <= 1024) // a kb
            {
                Span<byte> buff = stackalloc byte[totalBytes];
                Encoding.UTF8.GetBytes(str, buff);
                writer.BaseStream.Write(buff);
            }
            else
            {
                var bufferSize = Encoding.UTF8.GetMaxByteCount(chunkSize);
                var byteBuffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                try
                {
                    int offset = 0;
                    var sourceSpan = str.AsSpan();

                    while (offset < str.Length)
                    {
                        int charsToProcess = Math.Min(chunkSize, str.Length - offset);
                        int bytesWritten = Encoding.UTF8.GetBytes(sourceSpan.Slice(offset, charsToProcess), byteBuffer);
                        writer.BaseStream.Write(byteBuffer.AsSpan(0, bytesWritten));
                        offset += charsToProcess;
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(byteBuffer);
                }
            }
        }

        internal static void WriteSpan(BinaryWriter writer, bool[] values)
        {
            var count = values.Length;
            var buffer = ArrayPool<byte>.Shared.Rent(count);
            try
            {
                var dst = buffer.AsSpan(0, count);
                for (int i = 0; i < count; i++)
                {
                    dst[i] = BoolToByte(values[i]);
                }
                writer.Write(dst);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        internal static byte BoolToByte(bool value)
        {
            return value ? (byte)1 : (byte)0;
        }
        internal static void WriteBool(BinaryWriter writer, bool value)
        {
            writer.Write(BoolToByte(value));
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
    }
}
