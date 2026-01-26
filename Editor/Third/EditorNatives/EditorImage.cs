using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public enum EditorImageWriteFormat
    {
        Png,
        Jpg,
        Tga,
        Bmp,
        hdr
    }
    internal static class EditorImage
    {
        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Resize(byte[] inImageData, int channels, int inWidth, int inHeight, byte[] outImageData, int outWidth, int outHeight);

        [DllImport(EditorNatives.EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void STBI_Write(string path, int width, int height, int channels, IntPtr data, int format, int bitsPerChannel, byte flipVertical);

        public static unsafe void Write(string path, int width, int height, int channels, byte[] data, EditorImageWriteFormat format, int bitsPerChannel = 8, bool flipVertical = false)
        {
            fixed (byte* ptr = data)
            {
                STBI_Write(path, width, height, channels, (IntPtr)ptr, (int)format, bitsPerChannel, (byte)(flipVertical ? 1 : 0));
            }
        }
    }
}
