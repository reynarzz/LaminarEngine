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
        private const string Layer_TITLE = "Layer";
        protected override void OnDraw(ProjectSettings settings)
        {
            ImGui.Text("Layers");
            EditorGuiFieldsResolver.DrawArrayField("##Layers_ARRAY", ref settings.LayerSettings.Layers, false, 64, Layer_TITLE, false, (index, width, item) =>
            {
                var textSize = ImGui.CalcTextSize($"{Layer_TITLE} {index}");
                ImGui.Dummy(new Vector2(-textSize.X + 50, 0));
                ImGui.SameLine();
                ref var str = ref settings.LayerSettings.Layers[index];
                return EditorGuiFieldsResolver.DrawStringField($"##_LAYER__{index}", ref str);
            }, null);
        }
    }
}
