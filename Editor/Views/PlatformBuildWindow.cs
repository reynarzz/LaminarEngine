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
    internal class PlatformBuildWindow : EditorWindow
    {
        private int _selectedPlatform = 0;
       
        private float _leftPaneWidth = 150f;
        private const float SplitterWidth = 6f;
        private readonly string[] _platforms =
       {
            "Windows",
            "Linux",
            "macOS",
            "Android",
            "iOS"
        };

        public PlatformBuildWindow() : base("Window/Build")
        {
            
        }
        public override void OnDraw()
        {
            if(OnBeginWindow("Build", ImGuiWindowFlags.Modal, true))
            {
                var contentAvail = ImGui.GetContentRegionAvail();

                LeftPane(contentAvail);

                ImGui.SameLine();
                Splitter(contentAvail);
                ImGui.SameLine();

                RightPane(contentAvail);
            }

            OnEndWindow();
        }

        private void Splitter(Vector2 contentAvail)
        {

            var splitterId = ImGui.GetID("##Splitter");
            var splitterSize = new Vector2(SplitterWidth, contentAvail.Y);

            ImGui.InvisibleButton("##Splitter", splitterSize);

            bool hovered = ImGui.IsItemHovered();
            bool active = ImGui.IsItemActive();

            if (hovered || active)
            {
                ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeEW);
            }

            if (active)
            {
                _leftPaneWidth += ImGui.GetIO().MouseDelta.X;
                _leftPaneWidth = Math.Clamp(_leftPaneWidth, 80f, contentAvail.X - 80f);
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
            ImGui.BeginChild("PlatformsPane", new System.Numerics.Vector2(_leftPaneWidth, contentAvail.Y), ImGuiChildFlags.None);

            ImGui.Text("Platforms");
            ImGui.Separator();

            for (int i = 0; i < _platforms.Length; i++)
            {
                if (ImGui.Selectable(_platforms[i], _selectedPlatform == i))
                {
                    _selectedPlatform = i;
                }
            }

            ImGui.EndChild();

        }
        private void RightPane(Vector2 contentAvail)
        {
            ImGui.BeginChild("ConfigPane", new Vector2(0, contentAvail.Y), ImGuiChildFlags.None);

            ImGui.Text($"Configuration: {_platforms[_selectedPlatform]}");
            ImGui.Separator();

            ImGui.Checkbox("Enable Build", ref _dummyBool);
            ImGui.SliderInt("Optimization Level", ref _dummyInt, 0, 3);
            ImGui.InputText("Output Path", ref _dummyString, 256);

            if (ImGui.Button("Build android test"))
            {
                BuildSystem.BuildAsync(PlatformBuild.Android);
            }
            ImGui.EndChild();
        }

        // Dummy config values
        private bool _dummyBool = true;
        private int _dummyInt = 2;
        private string _dummyString = "build/output";

    }
}
