using Editor.Build;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    // TODO: expand it to support any kind of tasks display.
    internal class TaskWindow : EditorWindow
    {
        public override void OnDraw()
        {
            if (!BuildSystem.IsBuilding(PlatformBuild.GameAppDomain) && BuildSystem.IsAnyBuilding)
            {
                ImGui.SetNextWindowSize(new Vector2(500, 100));
                ImGui.SetNextWindowSizeConstraints(new Vector2(500, 100), new Vector2(500, 100));
                ImGui.Begin("Build##TaskWindow", ImGuiWindowFlags.NoDocking);
                ImGui.TextWrapped(BuildLogger.CurrentStatus);
                ImGui.End();
            }
        }
    }
}
