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
        public static void Image(nint image, vec2 imageSize)
        {
            ImGui.Image(image, new Vector2(imageSize.x, imageSize.y), new Vector2(0, 1), new Vector2(1, 0));
        }

        public static bool ImageButton(string id, nint image, vec2 imageSize)
        {
            return ImGui.ImageButton(id, image, new Vector2(imageSize.x, imageSize.y), new Vector2(0, 1), new Vector2(1, 0));
        }
    }
}
