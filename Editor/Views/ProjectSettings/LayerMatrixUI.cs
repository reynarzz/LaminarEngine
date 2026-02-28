using Engine;
using ImGuiNET;
using System;
using System.Numerics;

namespace Editor.Views
{
    public class LayerMatrixUI
    {
        private string[] _layerNames;

        public string[] Layers
        {
            get => _layerNames;
            set
            {
                if (value != _layerNames || value.Length != _layerNames.Length)
                {
                    _layerNames = value;
                }
            }
        }

        public bool[,] Matrix { get; set; }

        public void SetAll(bool value)
        {
            for (int i = 0; i < _layerNames.Length; i++)
            {
                for (int j = 0; j < _layerNames.Length; j++)
                {
                    Matrix[i, j] = value;
                }
            }
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
            float headerHeight = GetRotatedHeaderHeight();

            ImGui.TableNextRow(ImGuiTableRowFlags.None, headerHeight + 20);

            ImGui.TableSetColumnIndex(0);
            ImGui.Text("");

            int colIndex = 0;
            for (int col = 0; col < count; col++)
            {
                var name = _layerNames[col];
                if (string.IsNullOrEmpty(name))
                    continue;

                ImGui.TableSetColumnIndex(colIndex + 1);
                colIndex++;

                DrawRotatedText(name, 90.0f * Mathf.Deg2Rad, headerHeight);
            }
        }

        private float GetRotatedHeaderHeight()
        {
            float max = 0;
            for (int i = 0; i < _layerNames.Length; i++)
            {
                max = Mathf.Max(max, ImGui.CalcTextSize(_layerNames[i]).X);
            }
            return max;
        }

        private void DrawRows(int count)
        {
            for (int row = 0; row < count; row++)
            {
                var rowName = _layerNames[row];
                if (string.IsNullOrEmpty(rowName))
                    continue;

                ImGui.TableNextRow();

                ImGui.TableSetColumnIndex(0);
                ImGui.Text(rowName);

                int colTable = 0;

                for (int col = 0; col < count; col++)
                {
                    var colName = _layerNames[col];
                    if (string.IsNullOrEmpty(colName))
                        continue;

                    ImGui.TableSetColumnIndex(colTable + 1);
                    colTable++;

                    ImGui.PushID(row * 1000 + col);

                    if (col > row)
                    {
                        ImGui.Text("");
                    }
                    else if (col == row)
                    {
                        bool value = Matrix[row, col];
                        if (ImGui.Checkbox("##cell", ref value))
                            Matrix[row, col] = value;
                    }
                    else
                    {
                        bool value = Matrix[row, col];
                        if (ImGui.Checkbox("##cell", ref value))
                        {
                            Matrix[row, col] = value;
                            Matrix[col, row] = value;
                        }
                    }

                    ImGui.PopID();
                }
            }
        }


        private void DrawRotatedText(string text, float angleRad, float headerHeight)
        {
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            Vector2 cellMin = ImGui.GetCursorScreenPos();
            float columnWidth = ImGui.GetColumnWidth();

            var textSize = ImGui.CalcTextSize(text);

            var center = new Vector2(cellMin.X + columnWidth - (textSize.Y * 0.5f),
                                     cellMin.Y + headerHeight - textSize.X);

            drawList.PushClipRectFullScreen();

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
                return;

            Vector2 center = drawList.VtxBuffer[vtxStart].pos;

            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

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
}