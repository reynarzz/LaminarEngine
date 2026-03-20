using Engine;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    internal class SpriteInspectorDrawer : EditorDrawerBase<Sprite>
    {
        protected override bool AutoDrawTitle => true;

        protected override TitleIconInfo GetTitleIcon(Sprite target)
        {
            var cell = target.GetAtlasCell();
            return new TitleIconInfo()
            {
                Texture = target.Texture,
                Uvs = cell.Uvs,
                Size = new vec2(cell.Width, cell.Height)
            };
        }
        protected override void OnDraw(Sprite target)
        {
            ImGui.Text("Sprite: " + target.Name);
        }
    }
}
