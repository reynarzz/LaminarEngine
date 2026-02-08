using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal static class EditorUtils
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
    }
}
