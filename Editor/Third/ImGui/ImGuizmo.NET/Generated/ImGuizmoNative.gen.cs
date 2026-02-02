using System;
using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;

namespace ImGuizmoNET
{
    public static unsafe partial class ImGuizmoNative
    {
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_AllowAxisFlip(byte value);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_BeginFrame();
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_DecomposeMatrixToComponents(float* matrix, float* translation, float* rotation, float* scale);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_DrawCubes(float* view, float* projection, float* matrices, int matrixCount);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_DrawGrid(float* view, float* projection, float* matrix, float gridSize);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_Enable(byte enable);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuizmo_IsOverNil();
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuizmo_IsOverOPERATION(OPERATION op);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuizmo_IsUsing();
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern byte ImGuizmo_Manipulate(float* view, float* projection, OPERATION operation, MODE mode, float* matrix, float* deltaMatrix, float* snap, float* localBounds, float* boundsSnap);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_RecomposeMatrixFromComponents(float* translation, float* rotation, float* scale, float* matrix);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetDrawlist(ImDrawList* drawlist);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetGizmoSizeClipSpace(float value);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetID(int id);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetImGuiContext(IntPtr ctx);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetOrthographic(byte isOrthographic);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_SetRect(float x, float y, float width, float height);
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ImGuizmo_ViewManipulate_Float(float* view, float length, Vector2 position, Vector2 size, uint backgroundColor);
         
        // Custom:
        [DllImport("EditorNatives", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGizmoPixelSize(float size);
    }
}
