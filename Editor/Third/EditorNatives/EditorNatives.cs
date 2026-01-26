using System;
using System.Runtime.InteropServices;

namespace Editor
{
    internal class EditorNatives
    {
        public const string EDITOR_NATIVES = "EditorNatives";

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LogCallback(IntPtr message);

        [DllImport(EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitImAllGui();

        [DllImport(EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetCurrentWindowHitTestHole(float posX, float posY, float sizeX, float sizeY);

        [DllImport(EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetCurrentWindowDrawList();

        [DllImport(EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void imgui_NewFrame();

        [DllImport(EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterLogCallback(LogCallback callback);

        [DllImport(EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitGLFWImguiInternal(IntPtr windowPtr);

        [DllImport(EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void BeginGLFWImguiInternal();
        
        [DllImport(EDITOR_NATIVES, CallingConvention = CallingConvention.Cdecl)]
        public static extern void EndGLFWImguiInternal();
    }
}