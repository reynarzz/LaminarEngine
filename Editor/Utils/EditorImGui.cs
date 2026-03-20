using Engine;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal partial class EditorImGui
    {
        private const float POPUP_WINDOW_PADDING = 7.0f;

        public static void Image(nint image, ivec2 imageSize, ivec2 maxSize)
        {
            Image(image, imageSize, maxSize, null);
        }
        public static void Image(nint image, ivec2 imageSize, ivec2 maxSize, QuadUV? uvs)
        {
            var cursor = ImGui.GetCursorScreenPos();

            var width = imageSize.x == 0 ? maxSize.x : imageSize.x;
            var height = imageSize.y == 0 ? maxSize.y : imageSize.y;
            var scale = Mathf.Min((float)maxSize.x / width, (float)maxSize.y / height);

            var scaledW = (float)width * scale;
            var scaledH = (float)height * scale;

            var offsetY = ((float)maxSize.y - scaledH) * 0.5f;

            if (uvs == null)
            {
                ImGui.SetCursorScreenPos(new Vector2(cursor.X, cursor.Y + offsetY));
                Image(image, new vec2(scaledW, scaledH));
            }
            else
            {
                Image(image, new vec2(scaledW, scaledH), uvs.Value.BottomLeftUV, uvs.Value.TopLeftUV, uvs.Value.TopRightUV, 
                      uvs.Value.BottomRightUV, new vec2(cursor.X, cursor.Y + offsetY));
            }

            ImGui.SetCursorScreenPos(cursor);

        }
        public static void Image(nint image, vec2 imageSize)
        {
            Image(image, imageSize, new vec4(1, 1, 1, 1));
        }
        public static void Image(nint image, vec2 imageSize, vec4 tint)
        {
            ImGui.Image(image, new Vector2(imageSize.x, imageSize.y), new Vector2(0, 1), new Vector2(1, 0), new Vector4(tint.x, tint.y, tint.z, tint.w));
        }
        public static void ImageFromIcon(EditorIcon icon, vec2 imageSize)
        {
            Image(EditorTextureDatabase.GetIconImGui(icon), imageSize, new vec4(1, 1, 1, 1));
        }
        public static void Image(nint texture, vec2 size, in QuadUV uvs)
        {
            Image(texture, size, uvs.BottomLeftUV, uvs.TopLeftUV, uvs.TopRightUV, uvs.BottomRightUV);
        }
        public static void Image(nint texture, vec2 size, vec2 uvBottomLeft, vec2 uvTopLeft, vec2 uvTopRight, vec2 uvBottomRight)
        {
            var cursor = ImGui.GetCursorScreenPos();
            Image(texture, size, uvBottomLeft, uvTopLeft, uvTopRight, uvBottomRight, cursor.ToVec2());
        }
        public static void Image(nint texture, vec2 size, vec2 uvBottomLeft, vec2 uvTopLeft, vec2 uvTopRight, vec2 uvBottomRight, vec2 cursor)
        {
            var p1 = cursor.ToVector2();
            var p2 = new Vector2(cursor.x + size.x, cursor.y);
            var p3 = new Vector2(cursor.x + size.x, cursor.y + size.y);
            var p4 = new Vector2(cursor.x, cursor.y + size.y);

            var draw = ImGui.GetWindowDrawList();

            draw.AddImageQuad(texture, p1, p2, p3, p4, uvTopLeft.ToVector2(), uvTopRight.ToVector2(),
                              uvBottomRight.ToVector2(), uvBottomLeft.ToVector2());

            ImGui.Dummy(size.x, size.y);
        }
        public static void ImageFromIcon(EditorIcon icon, vec2 imageSize, vec4 tint)
        {
            Image(EditorTextureDatabase.GetIconImGui(icon), imageSize, tint);
        }
        public static bool ImageButton(string id, nint image, vec2 imageSize)
        {
            return ImGui.ImageButton(id, image, new Vector2(imageSize.x, imageSize.y), new Vector2(0, 1), new Vector2(1, 0));
        }

        public static bool ImageButtonFromIcon(string id, EditorIcon icon, vec2 imageSize)
        {
            return ImageButton(id, EditorTextureDatabase.GetIconImGui(icon), imageSize);
        }

        public static bool BeginPopup(string name, bool openWithRightClick = false)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(POPUP_WINDOW_PADDING, POPUP_WINDOW_PADDING));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(POPUP_WINDOW_PADDING, POPUP_WINDOW_PADDING));

            if (openWithRightClick)
            {
                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    ImGui.OpenPopup(name);
                }
            }

            var result = ImGui.BeginPopup(name);
            ImGui.PopStyleVar(2);
            return result;
        }

        internal static bool BeginPopupContextItem(string id)
        {
            var spacing = 15.0f;
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(POPUP_WINDOW_PADDING, POPUP_WINDOW_PADDING));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(POPUP_WINDOW_PADDING, POPUP_WINDOW_PADDING));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing, new Vector2(spacing, spacing));
            ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, spacing);
            var result = ImGui.BeginPopupContextItem(id);
            ImGui.PopStyleVar(4);

            return result;
        }
    }
}
