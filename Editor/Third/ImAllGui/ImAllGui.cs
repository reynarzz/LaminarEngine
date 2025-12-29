using System;
using System.Runtime.InteropServices;

namespace Editor
{
    internal class ImAllGui
    {
        [DllImport("ImAllGui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitImAllGui();
    }
}
