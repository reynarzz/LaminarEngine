using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class ProjectWizardWindow : EditorWindow
    {
        public override void OnDraw()
        {
            ImGui.Begin("Project");
            if (ImGui.Button("Open project"))
            {
                if (EditorFileDialog.PickFolder("C:/", out var selected))
                {
                    Debug.Log(selected);
                }
            }

            ImGui.End();
        }
    }
}