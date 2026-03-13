using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    internal class TextInspectorDrawer : EditorDrawerBase<TextAsset>
    {
        protected override bool AutoDrawTitle => true;

        protected override void OnDraw(TextAsset target)
        {
            ImGui.TextWrapped(target.Text);
        }
    }
}
