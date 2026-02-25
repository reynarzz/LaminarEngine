using Editor;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    [PropertyDrawer(nameof(TilemapRenderer.Options))]
    internal class TilemapPropertyDrawer : PropertyDrawer<TilemapRenderer, TilemapRenderingOptions>
    {
        protected internal override bool OnDrawProperty(Type type, string name, object target, in TilemapRenderingOptions valueIn, 
                                                        out object valueOut, Func<bool> defaultPropertyDrawer)
        {
            valueOut = valueIn;
            ImGui.Text("This should be a property");
            return defaultPropertyDrawer();
        }
    }
}