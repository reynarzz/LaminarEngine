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

            for (int i = 0; i < settings.LayerSettings.Layers.Length; i++)
            {
                ImGui.BeginDisabled(i == 0);
                var title = $"{Layer_TITLE} {i}";
                ImGui.Text(title);
                ImGui.SameLine();
                var textSize = ImGui.CalcTextSize(title);
                ImGui.Dummy(new Vector2(-textSize.X + 50, 0));
                ImGui.SameLine();
                EditorGuiFieldsResolver.DrawStringField($"##_LAYER__{i}", ref settings.LayerSettings.Layers[i]);
                ImGui.EndDisabled();
            }
        }
    }
}