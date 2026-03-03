using Engine;
using Engine.Data;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class PhysicsSettingsDrawer : ProjectMenuDrawer
    {
        private LayerMatrixUI _gridUI;

        public PhysicsSettingsDrawer()
        {
            _gridUI = new LayerMatrixUI();
        }
        protected override void OnDraw(ProjectSettings settings)
        {
            PropertiesGUIDrawEditor.DrawObject("Physics_Settings", settings.Physics);

            var length = settings.LayerSettings.Layers.Length;

            _gridUI.Matrix = settings.Physics.CollisionMatrix ?? new bool[length * (length - 1) / 2];
            _gridUI.Layers = settings.LayerSettings.Layers;

            ImGui.Text("Collision Matrix");
            if (_gridUI.Draw())
            { 
                LayerMask.UpdateLayers(_gridUI.Matrix, settings.LayerSettings.Layers);
            }
            if(ImGui.Button("Set layers"))
            {
                _gridUI.Matrix = LayerMask.BuildMatrixFromMasks();
            }
            if (ImGui.Button("Enable all"))
            {
                _gridUI.SetAll(true);
                LayerMask.UpdateLayers(_gridUI.Matrix, settings.LayerSettings.Layers);
            }
            ImGui.SameLine();
            if (ImGui.Button("Disable all"))
            {
                _gridUI.SetAll(false);
                LayerMask.UpdateLayers(_gridUI.Matrix, settings.LayerSettings.Layers);
            }
            settings.Physics.CollisionMatrix = _gridUI.Matrix;
        }
    }
}
