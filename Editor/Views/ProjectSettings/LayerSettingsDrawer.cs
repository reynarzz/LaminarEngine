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
            PropertiesGUIDrawEditor.DrawObject("Layer_Settings", settings.LayerSettings);
        }
    }
}
