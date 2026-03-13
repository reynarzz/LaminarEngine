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
    internal class EditorImGui
    {
        private const float POPUP_WINDOW_PADDING = 7.0f;
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
