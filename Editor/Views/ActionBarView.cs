using Editor.Build;
using Editor.Layers;
using Editor.Utils;
using Engine;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class ActionBarView : EditorWindow
    {
        private bool _shouldPlay = false;
        private bool _shouldPause = false;
        private bool _shouldSkip = false;
        private bool _shouldStop = false;
        
        private readonly vec2 _buttonSize = new vec2(20, 20);
        private const float _barHeight = 38;

        public override void OnDraw()
        {
            var viewport = ImGui.GetMainViewport();
            ImGui.SetNextWindowPos(viewport.Pos + new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(viewport.Size.X, _barHeight));

            var flags = ImGuiWindowFlags.NoTitleBar |
                                     ImGuiWindowFlags.NoCollapse |
                                     ImGuiWindowFlags.NoResize |
                                     ImGuiWindowFlags.NoMove |
                                     ImGuiWindowFlags.NoBringToFrontOnFocus |
                                     ImGuiWindowFlags.NoNavFocus |
                                     ImGuiWindowFlags.NoDocking |  
                                     ImGuiWindowFlags.NoSavedSettings;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.13f, 0.13f, 0.13f, 1.0f));
            OnBeginWindow("ActionBarView", flags, false);
            ImGui.BeginDisabled((BuildSystem.IsAnyBuilding && !Application.IsInPlayMode) /*|| BuildSystem.IsError*/);

            var playIcon = Application.IsInPlayMode ? EditorTextureDatabase.GetIconImGui(Utils.EditorIcon.Stop) :
                                                      EditorTextureDatabase.GetIconImGui(Utils.EditorIcon.Play);
            ImGui.SetCursorPosX(viewport.Size.X / 2.0f - (_buttonSize.x * 3.0f / 2.0f) - (ImGui.GetStyle().ItemSpacing.X * 3.0f));
            ImGui.SetCursorPosY(_barHeight / 2.0f - _buttonSize.y / 2.0f - ImGui.GetStyle().FramePadding.Y);

            unsafe
            {
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, *ImGui.GetStyleColorVec4(ImGuiCol.ButtonHovered));
            }

            if (Application.IsInPlayMode)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, EditorColors.MainColor.ToVector4());
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, EditorColors.MainColor.ToVector4());
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, EditorColors.MainColor.ToVector4());
            }

            if (EditorImGui.ImageButton("##Play", playIcon, _buttonSize))
            {
                if (!Application.IsInPlayMode)
                {
                    _shouldPlay = true;
                }
                else
                {
                    _shouldStop = true;
                }
            }
            if (Application.IsInPlayMode)
            {
                ImGui.PopStyleColor(3);
            }

            ImGui.SameLine();
            if (EditorImGui.ImageButton("##Pause", EditorTextureDatabase.GetIconImGui(Utils.EditorIcon.Pause), _buttonSize))
            {
                _shouldPause = true;
            }
            ImGui.SameLine();
            ImGui.BeginDisabled(!Application.IsInPlayMode);
            if (EditorImGui.ImageButton("##Skip", EditorTextureDatabase.GetIconImGui(Utils.EditorIcon.Skip), _buttonSize))
            {
                _shouldSkip = true;
            }
            ImGui.PopStyleColor();
            ImGui.EndDisabled();
            ImGui.EndDisabled();

            OnEndWindow();
            ImGui.PopStyleColor();

            ImGui.PopStyleVar(4);
        }

        public override void OnUpdate()
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
    }
}
