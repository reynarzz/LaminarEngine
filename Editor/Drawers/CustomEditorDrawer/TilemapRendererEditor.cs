using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    internal class TilemapRendererEditor : ComponentDrawer<TilemapRenderer>
    {
        protected internal override void OnDrawInspector(TilemapRenderer target, Action defaultDrawer)
        {
            //ImGui.Text("Custom editor here!");
            defaultDrawer();
        }
    }
}
