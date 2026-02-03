using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal static class EditorFileDialog
    {
        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void PushMenu(string path, IntPtr callback);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        private static extern void DisplayFolder(string path, byte highlight);

        internal static void DisplayFolder(string path, bool highlight)
        {
            DisplayFolder(path, EditorNativesUtils.ToByte(highlight));
        } 
         
        internal static void DisplayFolder(string path)
        {
            DisplayFolder(path, 0);
        }
    }
}