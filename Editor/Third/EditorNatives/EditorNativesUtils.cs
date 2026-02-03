using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
