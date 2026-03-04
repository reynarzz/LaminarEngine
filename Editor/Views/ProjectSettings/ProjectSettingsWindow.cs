
using Editor.Build;
using Editor.Data;
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
    public enum ProjectSettingsMenuType
    {
        Layers,
        Scenes,
        Physics
    }

    internal class ProjectSettingsWindow : EditorWindow
    {

        private int _selectedSettings = 2;

        private float _leftPaneWidth = 150f;
        private const float SplitterWidth = 6f;
        private float _splitterStartWidth;
        private float _splitterAccumulatedDelta;

        private readonly string[] _settingsNames;
        private Dictionary<ProjectSettingsMenuType, ProjectMenuDrawer> _drawers;

        public ProjectSettingsWindow() : base("Window/Project Settings")
        {
            _settingsNames = Enum.GetNames<ProjectSettingsMenuType>().ToArray();

            _drawers = new()
            {
                { ProjectSettingsMenuType.Physics, new PhysicsSettingsDrawer() },
                { ProjectSettingsMenuType.Layers, new LayerSettingsDrawer() },
                { ProjectSettingsMenuType.Scenes, new ScenesSettingsDrawer() }
            };
        }

        public override void OnDraw()
        {
            ImGui.BeginDisabled(BuildSystem.IsAnyBuilding);
            if (OnBeginWindow("Project Settings", ImGuiWindowFlags.Modal, true))
            {
                var contentAvail = ImGui.GetContentRegionAvail();

                LeftPane(contentAvail);

                ImGui.SameLine();
                Splitter(contentAvail);
                ImGui.SameLine();

                RightPane(contentAvail);
            }

            OnEndWindow();
            ImGui.EndDisabled();

        }

        private void Splitter(Vector2 contentAvail)
        {
            var splitterSize = new Vector2(SplitterWidth, Mathf.Clamp(contentAvail.Y, 1, contentAvail.Y + 1));
            ImGui.InvisibleButton("##Splitter", splitterSize);

            bool hovered = ImGui.IsItemHovered();
            bool active = ImGui.IsItemActive();

            if (hovered || active)
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
            }

            if (ImGui.IsItemActivated())
            {
                _splitterStartWidth = _leftPaneWidth;
                _splitterAccumulatedDelta = 0.0f;
            }

            if (active)
            {
                _splitterAccumulatedDelta += ImGui.GetIO().MouseDelta.X;

                float newWidth = _splitterStartWidth + _splitterAccumulatedDelta;
                _leftPaneWidth = Math.Clamp(newWidth, 80f, contentAvail.X - 80f);
            }

            var drawList = ImGui.GetWindowDrawList();
            var min = ImGui.GetItemRectMin();
            var max = ImGui.GetItemRectMax();

            uint splitterColor = ImGui.GetColorU32(ImGuiCol.Separator);
            if (hovered)
            {
                splitterColor = ImGui.GetColorU32(ImGuiCol.SeparatorHovered);
            }
            if (active)
            {
                splitterColor = ImGui.GetColorU32(ImGuiCol.SeparatorActive);
            }

            float centerX = (min.X + max.X) * 0.5f;
            drawList.AddLine(new Vector2(centerX, min.Y), new Vector2(centerX, max.Y), splitterColor, 1.0f);
        }

        private void LeftPane(Vector2 contentAvail)
        {
            ImGui.BeginChild("SettingsPane", new Vector2(_leftPaneWidth, contentAvail.Y), ImGuiChildFlags.None);

            ImGui.Text("Settings");
            ImGui.Separator();

            for (int i = 0; i < _settingsNames.Length; i++)
            {
                EditorImGui.Image(GetSettingsIcon((ProjectSettingsMenuType)i), new vec2(16, 16));
                ImGui.SameLine();
                if (ImGui.Selectable(_settingsNames[i], _selectedSettings == i))
                {
                    _selectedSettings = i;
                }
            }

            if (ImGui.Button("Save project settings"))
            {
                EditorDataManager.SaveProjectSettings();
            }
            ImGui.EndChild();
        }


        private void RightPane(Vector2 contentAvail)
        {
            ImGui.BeginChild("ConfigPane", new Vector2(0, contentAvail.Y), ImGuiChildFlags.None, ImGuiWindowFlags.NoMove |
                                                                                                 ImGuiWindowFlags.NoDocking);

            bool isValidSettings = false;
            if (_drawers.TryGetValue(GetSelectedSettingsMenu(), out var drawer))
            {
                isValidSettings = true;
            }

            EditorImGui.Image(GetSettingsIcon(GetSelectedSettingsMenu()), new vec2(32, 32));
            ImGui.SameLine();
            var title = GetSelectedSettingsMenu().ToString();
            var size = ImGui.CalcTextSize(title);
            var cursorY = ImGui.GetCursorPosY();

            ImGui.SetCursorPosY(cursorY + size.Y * 0.5f);
            ImGui.Text(title);
            if (isValidSettings)
            {
                ImGui.SameLine();
                var cursorPos = ImGui.GetCursorPos();

                ImGui.SetCursorPos(cursorPos.X + ImGui.GetContentRegionAvail().X - 146, cursorPos.Y + 4);
                if (ImGui.Button($"Save {title}"))
                {
                }
                //ImGui.SameLine();
                //cursorPos = ImGui.GetCursorPos();
                //ImGui.SetCursorPos(cursorPos.X, cursorPos.Y + 4);

                //if (ImGui.Button("Another"))
                //{
                //}
            }
            ImGui.Separator();

            ImGui.BeginChild("ProjectSettingsContent");
            if (isValidSettings)
            {
                drawer.OnDraw();
            }
            else
            {
                ImGui.Text($"Settings not implemented for '{title}'");
            }
            ImGui.EndChild();
            ImGui.EndChild();
        }

        private ProjectSettingsMenuType GetSelectedSettingsMenu()
        {
            return (ProjectSettingsMenuType)(_selectedSettings);
        }
        private nint GetSettingsIcon(ProjectSettingsMenuType menu)
        {
            switch (menu)
            {
                case ProjectSettingsMenuType.Physics:
                    return EditorTextureDatabase.GetIconImGui(EditorIcon.Physics);
                case ProjectSettingsMenuType.Layers:
                    return EditorTextureDatabase.GetIconImGui(EditorIcon.Actor);
                case ProjectSettingsMenuType.Scenes:
                    return EditorTextureDatabase.GetIconImGui(EditorIcon.Scene);
                default:
                    break;
            }

            return 0;
        }
    }
}
