using Editor.AssemblyHotReload;
using Editor.Layers;
using Engine;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class ActionBarView : IEditorWindow
    {
        private bool _shouldPlay = false;
        private bool _shouldPause = false;
        private bool _shouldStop = false;

        public void OnOpen()
        {
        }

        public void OnDraw()
        {
            var viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(viewport.Size.X, 33));

            var flags = ImGuiWindowFlags.NoTitleBar |
                                     ImGuiWindowFlags.NoCollapse |
                                     ImGuiWindowFlags.NoResize |
                                     ImGuiWindowFlags.NoMove |
                                     ImGuiWindowFlags.NoBringToFrontOnFocus |
                                     ImGuiWindowFlags.NoNavFocus |
                                     ImGuiWindowFlags.NoDocking;
            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.13f, 0.13f, 0.13f, 1.0f));
            ImGui.Begin("ActionBarView", flags);
            ImGui.BeginDisabled(GameAssemblyBuilder.IsBuilding && !Application.IsInPlayMode);
            if (ImGui.Button("Play"))
            {
                _shouldPlay = true;
            }
            ImGui.SameLine();
            if (ImGui.Button("Stop"))
            {
                _shouldStop = true;
            }
            ImGui.SameLine();
            if (ImGui.Button("Pause"))
            {
                _shouldPause = true;
            }
            ImGui.EndDisabled();
            ImGui.End();
            ImGui.PopStyleColor();

            ImGui.PopStyleVar(4);
        }

        public void OnUpdate()
        {
            // Handles events in the correct update stack.
            if (_shouldPlay)
            {
                _shouldPlay = false;
                PlaymodeController.Instance.PlayModeOn();

            }

            if (_shouldStop)
            {
                _shouldStop = false;
                PlaymodeController.Instance.PlayModeOff();

            }

            if (_shouldPause)
            {
                _shouldPause = false;
            }
        }

        public void OnClose()
        {
        }
    }
}
