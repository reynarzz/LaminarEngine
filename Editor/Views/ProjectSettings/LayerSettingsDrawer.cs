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
        private LayerMatrixUI _gridUI;
        private string[] _layersTest = ["Default", "UI", "Light", "Player", "Enemy", "Ground", "Floor"];
        public LayerSettingsDrawer()
        {
            _gridUI = new LayerMatrixUI();
        }

        protected override void OnDraw(ProjectSettings settings)
        {

            PropertiesGUIDrawEditor.DrawObject("Layer_Settings", settings.LayerSettings);
            // _gridUI.Layers = _layersTest;
            _gridUI.Layers = settings.LayerSettings.Layers;

            _gridUI.Draw();
        }
    }
}
