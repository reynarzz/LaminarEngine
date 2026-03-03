using Editor.Utils;
using Engine;
using Engine.Data;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class LayerSettingsDrawer : ProjectMenuDrawer
    {
        protected override void OnDraw(ProjectSettings settings)
        {
            ImGui.Text("Layers");
            EditorGuiFieldsResolver.DrawArrayField("##Layers_ARRAY", ref settings.LayerSettings.Layers, false, 64, "Layer", false, (index, width, item) =>
            {
                ref var str = ref settings.LayerSettings.Layers[index];
                return EditorGuiFieldsResolver.DrawStringField($"##_LAYER__{index}", ref str);
            }, null);
        }
    }
}
