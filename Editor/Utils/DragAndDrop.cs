using Editor.Cooker;
using Engine;
using Engine.Serialization;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal partial class EditorImGui
    {
        internal class DragAndDrop
        {
            public const string PAYLOAD_ID_EOBJECT = "EOBJECT_VALUE";
            // This is used so dear imgui registers a value, but we do not send complex values to dearImgui
            private static int _emptyDragValue = 0;

            [StructLayout(LayoutKind.Sequential)]
            internal struct ReferenceDragAndDropPayload
            {
                public Type Type;
                public Guid RefId;
                public int Index;
                public EObject Value;
            }

            private static ReferenceDragAndDropPayload _dragAndDropPayload;
            public static ReferenceDragAndDropPayload GetCurrentDropPayload()
            {
                return _dragAndDropPayload;
            }
            internal static void ItemDragReference(string title, nint image, string payloadId, EObject value, AssetType type, Guid refId)
            {
                ItemDragReference(title, image, payloadId, value, type.AssetTypeToType(), refId);
            }
            internal static void ItemDragReference(string title, string payloadId, EObject value, AssetType type, Guid refId)
            {
                ItemDragReference(title, payloadId, value, type.AssetTypeToType(), refId);
            }
            internal static void ItemDragReference(string title, string payloadId, EObject value, Type type, Guid refId)
            {
                var image = EditorTextureDatabase.GetIconImGui(type);
                ItemDragReference(title, image, payloadId, value, type, refId);
            }
            internal static void ItemDragReference(string title, nint image, string payloadId, EObject value, Type type, Guid refId)
            {
                unsafe
                {
                    if (ImGui.BeginDragDropSource())
                    {
                        _dragAndDropPayload = new ReferenceDragAndDropPayload()
                        {
                            RefId = refId,
                            Type = type,
                            Value = value,
                        };

                        fixed (int* ptr2 = &_emptyDragValue)
                        {
                            ImGui.SetDragDropPayload(payloadId, new IntPtr(ptr2), sizeof(uint));
                        }
                        var cursor = ImGui.GetCursorPos();

                        EditorImGui.Image(image, new vec2(16, 16));
                        ImGui.SameLine();
                        cursor.Y -= 2;
                        cursor.X += 20;
                        ImGui.SetCursorPos(cursor);
                        ImGui.Text(title);
                        ImGui.EndDragDropSource();
                    }
                }
            }

            internal static bool ItemDropReference(string payloadId, out ReferenceDragAndDropPayload value)
            {
                value = default;
                var result = false;

                unsafe
                {
                    if (ImGui.BeginDragDropTarget())
                    {
                        var ptr = ImGui.AcceptDragDropPayload(payloadId);
                        if (ptr.NativePtr != null)
                        {
                            value = GetCurrentDropPayload();
                            result = true;
                        }

                        ImGui.EndDragDropTarget();
                    }
                }

                return result;
            }
        }
    }
}
