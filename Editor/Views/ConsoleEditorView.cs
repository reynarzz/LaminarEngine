using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class ConsoleEditorView : IEditorWindow
    {
        private enum LogType
        {
            Log,
            Warning,
            Error
        }

        private struct LogEntry
        {
            public LogType Type;
            public string Message;
        }

        private static List<LogEntry> _entries = new List<LogEntry>();

        private bool _showLog = true;
        private bool _showWarning = true;
        private bool _showError = true;

        private float _splitterHeight = 120f;

        public static void AddLog(string msg)
        {
            _entries.Add(new LogEntry { Type = LogType.Log, Message = msg });
        }

        public static void AddWarning(string msg)
        {
            _entries.Add(new LogEntry { Type = LogType.Warning, Message = msg });
        }

        public static void AddError(string msg)
        {
            _entries.Add(new LogEntry { Type = LogType.Error, Message = msg });
        }

        public void OnOpen()
        {
        }

        public void OnClose()
        {
        }

        public void OnUpdate()
        {
        }

        public void OnDraw()
        {
            ImGui.Begin("Console");

            DrawToolbar();

            float availY = ImGui.GetContentRegionAvail().Y;
            if (availY <= 0)
            {
                ImGui.End();
                return;
            }

            float splitterThickness = 6f;
            float minPanelHeight = 10f;

            float maxSplitter = Math.Max(minPanelHeight, availY - minPanelHeight - splitterThickness);

            _splitterHeight = Math.Clamp(_splitterHeight, minPanelHeight, maxSplitter);

            DrawLogViewTop(_splitterHeight);
            DrawSplitter(splitterThickness);
            DrawLogViewBottom();

            ImGui.End();
        }
        private void DrawSplitter(float thickness)
        {
            ImGui.InvisibleButton("splitter", new Vector2(-1, thickness));

            if (ImGui.IsItemActive())
                _splitterHeight += ImGui.GetIO().MouseDelta.Y;
        }

        private void DrawLogViewTop(float height)
        {
            ImGui.BeginChild("TopPanel", new Vector2(0, height), ImGuiChildFlags.Borders);

            // ImGui.TextDisabled("Details");

            ImGui.EndChild();
        }
        private void DrawToolbar()
        {
            if (ImGui.Button("Clear"))
            {
                _entries.Clear();
            }

            ImGui.SameLine();
            ImGui.Spacing();
            ImGui.SameLine();

            float rightAlign = ImGui.GetWindowWidth() - 210;
            ImGui.SetCursorPosX(rightAlign);

            ToggleButton("Log", ref _showLog);
            ImGui.SameLine();
            ToggleButton("Warn", ref _showWarning);
            ImGui.SameLine();
            ToggleButton("Error", ref _showError);
        }

        private void ToggleButton(string label, ref bool value)
        {
            bool isValue = value;

            if (isValue)
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.35f, 0.35f, 0.35f, 1));

            if (ImGui.Button(label))
                value = !value;

            if (isValue)
                ImGui.PopStyleColor();
        }

        private void DrawSplitter()
        {
            var avail = ImGui.GetContentRegionAvail();

            ImGui.InvisibleButton("splitter", new Vector2(avail.X, 6));
            if (ImGui.IsItemActive())
                _splitterHeight += ImGui.GetIO().MouseDelta.Y;

            _splitterHeight = Math.Clamp(_splitterHeight, 50, avail.Y - 50);
        }

        private void DrawLogViewBottom()
        {
            ImGui.BeginChild("LogList", new Vector2(0, 0), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar);

            for (int i = 0; i < _entries.Count; i++)
            {
                var entry = _entries[i];
                if (!PassFilter(entry.Type))
                    continue;

                if (entry.Type == LogType.Warning)
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.8f, 0.2f, 1));
                else if (entry.Type == LogType.Error)
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0.3f, 0.3f, 1));

                ImGui.Text(entry.Message);

                if (entry.Type != LogType.Log)
                {
                    ImGui.PopStyleColor();
                }
            }

            if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
                ImGui.SetScrollHereY(1f);

            ImGui.EndChild();
        }

        private bool PassFilter(LogType type)
        {
            return type switch
            {
                LogType.Log => _showLog,
                LogType.Warning => _showWarning,
                LogType.Error => _showError,
                _ => true
            };
        }

    }
}
