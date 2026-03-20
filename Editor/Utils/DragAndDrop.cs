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
            private static readonly ivec2 _iconSize = new ivec2(16, 16);
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
            internal static bool ItemDragReference(string title, nint image, string payloadId, EObject value, AssetType type, Guid refId, 
                                                   int index, QuadUV? uvs, int width, int height)
            {
                return ItemDragReference(title, image, payloadId, value, type.AssetTypeToType(), refId, index, uvs, width, height);
            }
            internal static bool ItemDragReference(string title, nint image, string payloadId, EObject value, AssetType type, Guid refId, int index)
            {
                return ItemDragReference(title, image, payloadId, value, type.AssetTypeToType(), refId, index);
            }
            internal static bool ItemDragReference(string title, nint image, string payloadId, EObject value, AssetType type, Guid refId)
            {
                return ItemDragReference(title, image, payloadId, value, type.AssetTypeToType(), refId);
            }
            internal static bool ItemDragReference(string title, string payloadId, EObject value, AssetType type, Guid refId)
            {
                return ItemDragReference(title, payloadId, value, type.AssetTypeToType(), refId);
            }
            internal static bool ItemDragReference(string title, string payloadId, EObject value, Type type, Guid refId)
            {
                var image = EditorTextureDatabase.GetIconImGui(type);
                return ItemDragReference(title, image, payloadId, value, type, refId);
            }
            internal static bool ItemDragReference(string title, nint image, string payloadId, EObject value, Type type, Guid refId, int index = -1,
                                                   QuadUV? uvs = null, int width = 0, int height = 0)
            {
                unsafe
                {
                    var isDrag = ImGui.BeginDragDropSource();
                    if (isDrag)
                    {
                        _dragAndDropPayload = new ReferenceDragAndDropPayload()
                        {
                            RefId = refId,
                            Type = type,
                            Value = value,
                            Index = index
                        };

                        fixed (int* ptr2 = &_emptyDragValue)
                        {
                            ImGui.SetDragDropPayload(payloadId, new IntPtr(ptr2), sizeof(uint));
                        }
                        var cursor = ImGui.GetCursorPos();

                        width = width == 0 ? _iconSize.x : width;
                        height = height == 0 ? _iconSize.y : height;
                        var scale = Mathf.Min((float)_iconSize.x / width, (float)_iconSize.y / height);

                        var scaledW = width * scale;
                        var scaledH = height * scale;

                        var offsetY = (_iconSize.y - scaledH) * 0.5f;

                        ImGui.SetCursorPos(new Vector2(ImGui.GetCursorPosX(), cursor.Y + offsetY));

                        if (uvs == null)
                        {
                            EditorImGui.Image(image, new vec2(scaledW, scaledH));
                        }
                        else
                        {
                            EditorImGui.Image(image, new vec2(scaledW, scaledH), uvs.Value);
                        }
                        ImGui.SameLine();
                        cursor.Y -= 2;
                        cursor.X += 20;
                        ImGui.SetCursorPos(cursor);
                        ImGui.Text(title);
                        ImGui.EndDragDropSource();
                    }

                    return isDrag;
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
