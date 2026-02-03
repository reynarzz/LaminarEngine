using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal static class EditorNativesUtils
    {
        internal static bool ToBool(byte v)
        {
            return (v & 1) == 1;
        }

        internal static byte ToByte(bool v)
        {
            return (byte)(v ? 1 : 0);
        }

        internal static IntPtr AllocUtf8StringArray(string[] strings)
        {
            var ptrs = new IntPtr[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                ptrs[i] = Marshal.StringToCoTaskMemUTF8(strings[i]);
            }

            IntPtr array = Marshal.AllocHGlobal(IntPtr.Size * strings.Length);
            Marshal.Copy(ptrs, 0, array, ptrs.Length);

            return array;
        }

        internal static void FreeUtf8StringArray(IntPtr array, int count)
        {
            for (int i = 0; i < count; i++)
            {
                IntPtr strPtr = Marshal.ReadIntPtr(array, i * IntPtr.Size);
                Marshal.FreeCoTaskMem(strPtr);
            }

            Marshal.FreeHGlobal(array);
        }
    }
}
