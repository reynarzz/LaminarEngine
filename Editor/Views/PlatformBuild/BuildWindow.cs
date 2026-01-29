using Editor.Build;
using Editor.Data;
using Editor.Utils;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class BuildWindow : EditorWindow
    {
        private int _selectedPlatform = 0;

        private float _leftPaneWidth = 150f;
        private const float SplitterWidth = 6f;
        private float _splitterStartWidth;
        private float _splitterAccumulatedDelta;

        private readonly string[] _platforms;
        private Dictionary<PlatformBuild, PlatformBuildSettingsDrawer> _drawers;
        public BuildWindow() : base("Window/Build")
        {
            _platforms = Enum.GetNames<PlatformBuild>().Skip(1).ToArray();
            _drawers = new()
            {
                { PlatformBuild.Windows, new WindowsBuildDrawer() },
                { PlatformBuild.Android, new AndroidBuildDrawer() },

            };
        }

        public override void OnDraw()
        {
            ImGui.BeginDisabled(BuildSystem.IsAnyBuilding);
            if (OnBeginWindow("Build", ImGuiWindowFlags.Modal, true))
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
            var splitterSize = new Vector2(SplitterWidth, contentAvail.Y);
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
            ImGui.BeginChild("PlatformsPane", new Vector2(_leftPaneWidth, contentAvail.Y), ImGuiChildFlags.None);

            ImGui.Text("Platforms");
            ImGui.Separator();

            for (int i = 0; i < _platforms.Length; i++)
            {
                ImGui.Image(GetPlatformIcon((PlatformBuild)(i + 1)), new Vector2(16, 16), new Vector2(0, 1), new Vector2(1, 0));
                ImGui.SameLine();
                if (ImGui.Selectable(_platforms[i], _selectedPlatform == i))
                {
                    _selectedPlatform = i;
                }
            }

            if(ImGui.Button("Save all config test"))
            {
                EditorDataManager.SaveAll();
            }
            ImGui.EndChild();
        }

      
        private void RightPane(Vector2 contentAvail)
        {
            ImGui.BeginChild("ConfigPane", new Vector2(0, contentAvail.Y), ImGuiChildFlags.None, ImGuiWindowFlags.NoMove |
                                                                                                 ImGuiWindowFlags.NoDocking);

            bool isValidPlatformBuild = false;
            if (_drawers.TryGetValue(GetSelectedPlatform(), out var drawer))
            {
                isValidPlatformBuild = true;
            }

            ImGui.Image(GetPlatformIcon(GetSelectedPlatform()), new Vector2(32, 32), new Vector2(0, 1), new Vector2(1, 0));
            ImGui.SameLine();
            var title = GetSelectedPlatform().ToString();
            var size = ImGui.CalcTextSize(title);
            var cursorY = ImGui.GetCursorPosY();

            ImGui.SetCursorPosY(cursorY + size.Y * 0.5f);
            ImGui.Text(title);
            if (isValidPlatformBuild)
            {
                ImGui.SameLine();

                if (ImGui.Button($"Build {GetSelectedPlatform()}", new Vector2(100, 23)))
                {
                    BuildSystem.BuildAsync(GetSelectedPlatform());
                }

            }
            ImGui.Separator();

            if (isValidPlatformBuild)
            {
                drawer.OnDraw(EditorDataManager.BuildSettings.GetBuildSettings(GetSelectedPlatform()));
            }
            else
            {
                ImGui.Text("Build settings not implemented for this platform");
            }

            ImGui.EndChild();
        }

        private PlatformBuild GetSelectedPlatform()
        {
            return (PlatformBuild)(_selectedPlatform + 1);
        }
        private nint GetPlatformIcon(PlatformBuild platform)
        {
            switch (platform) 
            {
                case PlatformBuild.Windows:
                    return EditorTextureDatabase.GetIconImGui(EditorIcon.WindowsSmall);
                case PlatformBuild.MacOs:
                    return EditorTextureDatabase.GetIconImGui(EditorIcon.AppleSmall);
                case PlatformBuild.Android:
                    return EditorTextureDatabase.GetIconImGui(EditorIcon.AndroidSmall);
                case PlatformBuild.IOS:
                    return EditorTextureDatabase.GetIconImGui(EditorIcon.AppleSmall);
                case PlatformBuild.Linux:
                    return EditorTextureDatabase.GetIconImGui(EditorIcon.LinuxSmall);
                default:
                    break;
            }

            return 0;
        }
    }
}
