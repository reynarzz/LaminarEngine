using System;
using System.Runtime.InteropServices;

namespace Editor
{
    internal class ImAllGui
    {
        [DllImport("ImAllGui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitImAllGui();

        [DllImport("ImAllGui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCurrentWindowHitTestHole(float posX, float posY, float sizeX, float sizeY);

        [DllImport("ImAllGui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetCurrentWindowDrawList();
    }
}
