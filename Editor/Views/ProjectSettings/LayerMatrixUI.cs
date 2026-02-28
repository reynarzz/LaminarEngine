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
    public class LayerMatrixUI
    {
        private string[] _layerNames;
        private bool[,] _matrix;

        public string[] Layers
        {
            get => _layerNames;
            set
            {
                if (value != _layerNames || value.Length != _layerNames.Length)
                {
                    _layerNames = value;
                    _matrix = new bool[_layerNames.Length, _layerNames.Length];
                }
            }
        }

        public bool[,] Matrix
        {
            get { return _matrix; }
        }

        public void Draw()
        {
            int count = _layerNames.Length;

            if (count <= 0)
            {
                return;
            }
            var flags = ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.HighlightHoveredColumn;

            if (ImGui.BeginTable("LayerCollisionMatrix", count + 1, flags))
            {
                DrawHeaderRow(count);
                DrawRows(count);
                ImGui.EndTable();
            }
        }

        private void DrawHeaderRow(int count)
        {
            float headerHeight = GetMaxRotatedHeaderHeight();

            ImGui.TableNextRow(ImGuiTableRowFlags.None, headerHeight);

            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");

            int colIndex = 0;
            for (int col = 0; col < count; col++)
            {
                var name = _layerNames[col];
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }
                ImGui.TableSetColumnIndex(colIndex + 1);
                colIndex++;
                DrawRotatedText(name, 90.0f * Mathf.Deg2Rad, headerHeight);
            }
        }

        private float GetMaxRotatedHeaderHeight()
        {
            float max = 0f;

            for (int i = 0; i < _layerNames.Length; i++)
            {
                Vector2 size = ImGui.CalcTextSize(_layerNames[i]);
                max = MathF.Max(max, size.X);
            }

            return max + ImGui.GetStyle().CellPadding.Y * 2;
        }

        private void DrawRows(int count)
        {
            for (int row = 0; row < count; row++)
            {
                var rowName = _layerNames[row];
                if (string.IsNullOrEmpty(rowName))
                {
                    continue;
                }

                ImGui.TableNextRow();

                ImGui.TableSetColumnIndex(0);
                ImGui.Text(rowName);
                int colTable = 0;

                for (int col = 0; col < count; col++)
                {
                    var colName = _layerNames[col];
                    if (string.IsNullOrEmpty(colName))
                    {
                        continue;
                    }
                    ImGui.TableSetColumnIndex(colTable + 1);
                    colTable++;

                    ImGui.PushID(row * 1000 + col);

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

        private void DrawRotatedText(string text, float angleRad, float headerHeight)
        {
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            Vector2 textSize = ImGui.CalcTextSize(text);
            float columnWidth = ImGui.GetColumnWidth();

            Vector2 cursor = ImGui.GetCursorScreenPos();

            Vector2 center = new Vector2(cursor.X + columnWidth * 0.5f,
                                         cursor.Y + headerHeight * 0.5f);

            Vector2 clipMin = new Vector2(center.X - headerHeight, cursor.Y);
            Vector2 clipMax = new Vector2(center.X + headerHeight, cursor.Y + headerHeight);

            drawList.PushClipRect(clipMin, clipMax, true);
            AddTextRotated(drawList, ImGui.GetFont(), ImGui.GetFontSize(), center, ImGui.GetColorU32(ImGuiCol.Text), text, angleRad);
            drawList.PopClipRect();

            ImGui.Dummy(new Vector2(columnWidth, headerHeight));
        }

        public static unsafe void AddTextRotated(ImDrawListPtr drawList, ImFontPtr font, float fontSize, Vector2 pos, uint col, string text, float angle)
        {
            int vtxStart = drawList.VtxBuffer.Size;

            drawList.AddText(font, fontSize, pos, col, text);

            int vtxEnd = drawList.VtxBuffer.Size;

            if (vtxEnd == vtxStart)
            {
                return;
            }

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

                    vtx->pos = new Vector2(center.X + p.X * cos - p.Y * sin,
                                           center.Y + p.X * sin + p.Y * cos);
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
