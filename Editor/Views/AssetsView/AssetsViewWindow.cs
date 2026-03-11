using Editor.Build;
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
    internal class AssetsViewWindow : EditorWindow
    {
        private float _leftPaneWidth = 150f;
        private const float SplitterWidth = 6f;
        private float _splitterStartWidth;
        private float _splitterAccumulatedDelta;

        public AssetsViewWindow() : base("Window/Assets View")
        {
        }

        public override void OnDraw()
        {
            ImGui.BeginDisabled(BuildSystem.IsAnyBuilding);
            if (OnBeginWindow("Assets View", ImGuiWindowFlags.Modal, true))
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
            ImGui.BeginChild("Left Pane", new Vector2(_leftPaneWidth, contentAvail.Y), ImGuiChildFlags.None);

            ImGui.EndChild();
        }


        private void RightPane(Vector2 contentAvail)
        {
            ImGui.BeginChild("Right Pane", new Vector2(0, contentAvail.Y), ImGuiChildFlags.None, ImGuiWindowFlags.NoMove |
                                                                                                 ImGuiWindowFlags.NoDocking);

            ImGui.EndChild();
        }
    }
}