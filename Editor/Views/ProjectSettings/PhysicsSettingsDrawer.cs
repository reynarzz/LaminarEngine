using Engine.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class PhysicsSettingsDrawer : ProjectMenuDrawer
    {
        protected override void OnDraw(ProjectSettings settings)
        {
            PropertiesGUIDrawEditor.DrawObject("Physics_Settings", settings.Physics);
        }
    }
}
