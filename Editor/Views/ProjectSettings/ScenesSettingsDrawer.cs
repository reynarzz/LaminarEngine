using Editor.Utils;
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
    internal class ScenesSettingsDrawer : ProjectMenuDrawer
    {
        protected override void OnDraw(ProjectSettings settings)
        {
            // TODO: Draw from scratch

            PropertiesGUIDrawEditor.DrawObject("__Scene_Settings__", settings.SceneSettings);
        }
    }
}
