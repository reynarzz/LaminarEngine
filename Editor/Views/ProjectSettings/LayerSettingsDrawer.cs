using Engine;
using Engine.Data;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class LayerSettingsDrawer : ProjectMenuDrawer
    {
        private LayerMatrixUI _gridUI;

        public LayerSettingsDrawer()
        {
            _gridUI = new LayerMatrixUI(["Default", "UI", "Light", "Player", "Enemy", "Ground", "Floor"]);
        }

        protected override void OnDraw(ProjectSettings settings)
        {

            PropertiesGUIDrawEditor.DrawObject("Layer_Settings", settings.LayerSettings);

            _gridUI.Draw();
        }
    }

    public class LayerMatrixUI
    {
        private string[] _layerNames;
        private bool[,] _matrix;

        public LayerMatrixUI(string[] layerNames)
        {
            _layerNames = layerNames;
            _matrix = new bool[layerNames.Length, layerNames.Length];
        }

        public bool[,] Matrix
        {
            get { return _matrix; }
        }

        public void Draw()
        {
            int count = _layerNames.Length;

            ImGuiTableFlags flags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg |
                ImGuiTableFlags.SizingFixedFit |
                ImGuiTableFlags.NoPadInnerX;

            if (ImGui.BeginTable("LayerCollisionMatrix", count + 1, flags))
            {
                DrawHeaderRow(count);
                DrawRows(count);
                ImGui.EndTable();
            }
        }

        private void DrawHeaderRow(int count)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");

            for (int col = 0; col < count; col++)
            {
                ImGui.TableSetColumnIndex(col + 1);
                DrawRotatedText(_layerNames[col], 90.0f * Mathf.Deg2Rad);
            }
        }

        private void DrawRows(int count)
        {
            for (int row = 0; row < count; row++)
            {
                ImGui.TableNextRow();

                ImGui.TableSetColumnIndex(0);
                ImGui.Text(_layerNames[row]);

                for (int col = 0; col < count; col++)
                {
                    ImGui.TableSetColumnIndex(col + 1);

                    ImGui.PushID(row * 1000 + col);

                    // Skip upper triangle
                    if (col > row)
                    {
                        ImGui.Text("");
                    }
                    else if (col == row)
                    {
                        bool value = _matrix[row, col];

                        if (ImGui.Checkbox("##cell", ref value))
                        {
                            _matrix[row, col] = value;
                        }
                    }
                    // Lower triangle only
                    else
                    {
                        bool value = _matrix[row, col];

                        if (ImGui.Checkbox("##cell", ref value))
                        {
                            _matrix[row, col] = value;
                            _matrix[col, row] = value;
                        }
                    }

                    ImGui.PopID();
                }
            }
        }
        private void DrawRotatedText(string text, float angleRad)
        {
            Vector2 pos = ImGui.GetCursorScreenPos();
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            // Measure text size before rotation
            Vector2 textSize = ImGui.CalcTextSize(text);

            // Center inside the table cell
            float cellWidth = ImGui.GetColumnWidth();
            float cellHeight = ImGui.GetTextLineHeight() * text.Length;

            Vector2 pivot = new Vector2(
                pos.X + cellWidth * 0.5f,
                pos.Y + textSize.X * 0.5f
            );

            AddTextRotated(drawList, ImGui.GetFont(), ImGui.GetFontSize(), pivot, ImGui.GetColorU32(ImGuiCol.Text), text, angleRad);

            ImGui.Dummy(new Vector2(textSize.Y, textSize.X));
        }

        public static unsafe void AddTextRotated(ImDrawListPtr drawList, ImFontPtr font, float fontSize, Vector2 pos, uint col, string text, float angle)
        {
            int vtxStart = drawList.VtxBuffer.Size;

            drawList.AddText(font, fontSize, pos, col, text);

            int vtxEnd = drawList.VtxBuffer.Size;

            if (vtxEnd == vtxStart)
                return;

            Vector2 center = GetTextCenter(drawList, vtxStart, vtxEnd);

            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            unsafe
            {
                ImDrawVert* vtxBuffer = (ImDrawVert*)drawList.VtxBuffer.Data;

                for (int i = vtxStart; i < vtxEnd; i++)
                {
                    ImDrawVert* vtx = &vtxBuffer[i];

                    Vector2 p = vtx->pos - center;

                    vtx->pos = new Vector2(
                        center.X + p.X * cos - p.Y * sin,
                        center.Y + p.X * sin + p.Y * cos
                    );
                }
            }
        }


        private static unsafe Vector2 GetTextCenter(ImDrawListPtr drawList, int start, int end)
        {
            Vector2 min = drawList.VtxBuffer[start].pos;
            Vector2 max = min;

            for (int i = start + 1; i < end; i++)
            {
                var p = drawList.VtxBuffer[i].pos;

                min.X = MathF.Min(min.X, p.X);
                min.Y = MathF.Min(min.Y, p.Y);
                max.X = MathF.Max(max.X, p.X);
                max.Y = MathF.Max(max.Y, p.Y);
            }

            return (min + max) * 0.5f;
        }
    }
}
