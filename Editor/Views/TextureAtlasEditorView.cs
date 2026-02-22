using Editor.Build;
using Editor.Utils;
using Engine;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using Engine;
using System;
using System.Diagnostics;
using System.Numerics;

namespace Editor.Views
{
    internal class TextureAtlasEditorView : EditorDrawerBase<Texture2D>
    {
        protected override bool AutoDrawTitle => false;

        private float _zoomFactor = 1.0f;
        private const float _zoomSpeed = 0.1f;
        private const float _minZoom = 0.5f;
        private const float _maxZoom = 13.0f;
        private const float _pivotDotSize = 0.9f;

        private int _chunkIndex = 0;

        private vec2 _imageSize;
        private vec2 _zoomedSize;

        private ivec2 _sliceDim = new ivec2(8, 8);
        public bool _isOpen = false;

        private vec2 _panOffset;
        private bool _isPanning;
        private vec2 _lastMousePosLocal;
        private bool _panInitialized;

        private float _fitPadding = 32.0f;

        public bool IsWindowOpen() { return _isOpen; }
        private TextureMetaFile _meta;
        private Texture2D _text;
        internal void OnOpen(Texture2D texture, TextureMetaFile meta)
        {
            _chunkIndex = 0;
            _isOpen = true;
            _meta = meta;
            _text = texture;
            _imageSize = new vec2(texture.Width, texture.Height);
            _sliceDim = GetAtlasData().ChunksCount == 0 ? new ivec2(8, 8) :
                new ivec2(GetAtlasData().GetCell(0).Width, GetAtlasData().GetCell(0).Height);

            RestartView();


        }

        private void RestartView()
        {
            _zoomedSize = default;
            _panOffset = default;
            _isPanning = false;
            _panInitialized = false;
        }

        protected override void OnDraw(Texture2D texture)
        {
            if (!_isOpen)
                return;

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                && Input.GetKeyDown(KeyCode.F))
            {
                RestartView();
            }

            ImGui.Begin("Atlas Editor", ref _isOpen, ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoCollapse);
            ImGui.BeginDisabled(BuildSystem.IsAnyBuilding);
            var io = ImGui.GetIO();

            Vector2 canvasPos = ImGui.GetCursorScreenPos();
            Vector2 canvasSize = ImGui.GetContentRegionAvail();

            vec2 canvasPosV = canvasPos.ToVec2();
            vec2 canvasSizeV = canvasSize.ToVec2();

            vec2 mouseLocal = ImGui.GetMousePos().ToVec2() - canvasPosV;

            if (!_panInitialized)
            {
                _zoomFactor = ComputeFitZoom(canvasSizeV, _imageSize, _fitPadding);

                vec2 canvasCenter = canvasSizeV * 0.5f;
                vec2 imageCenter = (_imageSize * _zoomFactor) * 0.5f;

                _panOffset = canvasCenter - imageCenter;
                _panInitialized = true;
            }

            if (ImGui.IsWindowHovered())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Middle))
                {
                    _isPanning = true;
                    _lastMousePosLocal = mouseLocal;
                }

                if (_isPanning && ImGui.IsMouseDown(ImGuiMouseButton.Middle))
                {
                    vec2 delta = mouseLocal - _lastMousePosLocal;
                    _panOffset += delta;
                    _lastMousePosLocal = mouseLocal;
                }

                if (ImGui.IsMouseReleased(ImGuiMouseButton.Middle))
                {
                    _isPanning = false;
                }

                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    vec2 imageSpace = (mouseLocal - _panOffset) / _zoomFactor;
                    imageSpace.y = _imageSize.y - imageSpace.y;

                    if (imageSpace.x >= 0 && imageSpace.y >= 0 &&
                        imageSpace.x < _imageSize.x && imageSpace.y < _imageSize.y)
                    {
                        for (int i = 0; i < GetAtlasData().ChunksCount; i++)
                        {
                            var chunk = GetAtlasData().GetCell(i);

                            if (imageSpace.x >= chunk.XPixel &&
                                imageSpace.x < chunk.XPixel + chunk.Width &&
                                imageSpace.y >= chunk.YPixel &&
                                imageSpace.y < chunk.YPixel + chunk.Height)
                            {
                                _chunkIndex = i;
                                break;
                            }
                        }
                    }
                }
            }

            float wheel = io.MouseWheel;
            if (wheel != 0 && ImGui.IsWindowHovered())
            {
                vec2 mouseBefore = (mouseLocal - _panOffset) / _zoomFactor;

                _zoomFactor = Mathf.Clamp(_zoomFactor + wheel * _zoomSpeed, _minZoom, _maxZoom);

                vec2 mouseAfter = mouseBefore * _zoomFactor;
                _panOffset = mouseLocal - mouseAfter;
            }

            _zoomedSize = _imageSize * _zoomFactor;

            vec2 drawOrigin = canvasPosV + _panOffset;

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            drawList.AddImage(EditorTextureDatabase.GetIconImGui(texture), drawOrigin.ToVector2(),
                             (drawOrigin + _zoomedSize).ToVector2(), new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));

            DrawGrid(drawOrigin, texture, _zoomFactor);

            if (GetAtlasData().ChunksCount == 0)
            {
                DrawDottedRect(drawList, drawOrigin.ToVector2(), (drawOrigin + _zoomedSize).ToVector2(),
                               Color.White.ToARGB_U32(), 6.0f, 3.0f);
            }

            ImGui.SetNextWindowBgAlpha(0.9f);
            ImGui.SetNextWindowPos(ImGui.GetWindowPos() + new Vector2(10, 30), ImGuiCond.Always);


            PropertiesGUI(texture);

            ImGui.EndDisabled();
            ImGui.End();
        }

        private void DrawDottedRect(ImDrawListPtr drawList, Vector2 min, Vector2 max,
                                    uint color, float dashLength, float gapLength)
        {
            void DrawDottedLine(Vector2 a, Vector2 b)
            {
                Vector2 dir = Vector2.Normalize(b - a);
                float length = Vector2.Distance(a, b);
                float t = 0.0f;

                while (t < length)
                {
                    float seg = MathF.Min(dashLength, length - t);
                    Vector2 p1 = a + dir * t;
                    Vector2 p2 = a + dir * (t + seg);
                    drawList.AddLine(p1, p2, color);
                    t += dashLength + gapLength;
                }
            }

            DrawDottedLine(new Vector2(min.X, min.Y), new Vector2(max.X, min.Y));
            DrawDottedLine(new Vector2(max.X, min.Y), new Vector2(max.X, max.Y));
            DrawDottedLine(new Vector2(max.X, max.Y), new Vector2(min.X, max.Y));
            DrawDottedLine(new Vector2(min.X, max.Y), new Vector2(min.X, min.Y));
        }

        private float ComputeFitZoom(vec2 canvasSize, vec2 imageSize, float padding)
        {
            float availableW = Math.Max(1.0f, canvasSize.x - padding * 2.0f);
            float availableH = Math.Max(1.0f, canvasSize.y - padding * 2.0f);

            float zoomW = availableW / imageSize.x;
            float zoomH = availableH / imageSize.y;

            return Mathf.Clamp(Math.Min(zoomW, zoomH), _minZoom, _maxZoom);
        }

        private void PropertiesGUI(Texture2D texture)
        {
            const float OverlayWidth = 250.0f;
            const float OverlayHeight = 200.0f;
            const float Margin = 10.0f;

            Vector2 canvasPos = ImGui.GetCursorScreenPos();
            Vector2 canvasSize = ImGui.GetContentRegionAvail();

            Vector2 overlayPos = new Vector2(canvasPos.X + canvasSize.X - OverlayWidth - Margin,
                                             canvasPos.Y + canvasSize.Y - OverlayHeight - Margin);

            ImGui.SetCursorScreenPos(overlayPos);

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(18, 18));
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 0.35f));

            
            var flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

            if (ImGui.IsMouseDown(ImGuiMouseButton.Middle))
            {

                flags |= ImGuiWindowFlags.NoMouseInputs;
            }

            ImGui.BeginChild("AtlasPropertiesOverlay", new Vector2(OverlayWidth, OverlayHeight),
                             ImGuiChildFlags.Borders, flags);

            DrawProperties(texture);

            ImGui.EndChild();

            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
        }

        private void DrawGrid(vec2 origin, Texture2D tex, float zoom)
        {
            if (GetAtlasData().ChunksCount == 0)
                return;

            var nonSelectedChunkColor = new Color(1, 1, 1, 0.3f);

            for (int i = 0; i < GetAtlasData().ChunksCount; i++)
            {
                if (_chunkIndex != i)
                {
                    DrawChunkLines(GetAtlasData().GetCell(i), nonSelectedChunkColor);
                }
            }

            DrawChunkLines(GetAtlasData().GetCell(_chunkIndex), Color.White);

            void DrawChunkLines(TextureAtlasCell chunk, Color color)
            {
                ImDrawListPtr drawList = ImGui.GetWindowDrawList();

                float x1 = origin.x + chunk.XPixel * zoom;
                float x2 = origin.x + (chunk.XPixel + chunk.Width) * zoom;
                float y1 = origin.y + (_imageSize.y - chunk.YPixel - chunk.Height) * zoom;
                float y2 = origin.y + (_imageSize.y - chunk.YPixel) * zoom;

                drawList.AddLine(new Vector2(x1, y1), new Vector2(x1, y2), color.ToARGB_U32());
                drawList.AddLine(new Vector2(x2, y1), new Vector2(x2, y2), color.ToARGB_U32());
                drawList.AddLine(new Vector2(x1, y1), new Vector2(x2, y1), color.ToARGB_U32());
                drawList.AddLine(new Vector2(x1, y2), new Vector2(x2, y2), color.ToARGB_U32());

                float lerpX = Mathf.Lerp(x1, x2, chunk.Pivot.x);
                float lerpY = Mathf.Lerp(y1, y2, 1.0f - chunk.Pivot.y);

                var size = _pivotDotSize * _zoomFactor;
                drawList.AddRectFilled(new Vector2(lerpX - size, lerpY - size), new Vector2(lerpX + size, lerpY + size), Color.Green.ToARGB_U32());
            }
        }

        private void DrawProperties(Texture2D texture)
        {
            var atlasData = GetAtlasData();

            // ImGui.Text($"{texture.Name} ({texture.Width}, {texture.Height})");
            ImGui.Text("Slice size");
            ImGui.SameLine();
            if (EditorGuiFieldsResolver.DrawIVec2Field("#CellSizeVec", ref _sliceDim, 200))
            {
                _sliceDim.x = Mathf.Clamp(_sliceDim.x, 1, texture.Width);
                _sliceDim.y = Mathf.Clamp(_sliceDim.y, 1, texture.Height);
            }

            ImGui.SameLine();
            if (ImGui.Button("Slice"))
            {
                TextureAtlasUtils.SliceTiles(GetAtlasData(), _sliceDim.x, _sliceDim.y,
                                             texture.Width, texture.Height);
                _chunkIndex = 0;
            }

            ImGui.BeginDisabled(atlasData.ChunksCount == 0);
            ImGui.Separator();

            float cursorX = ImGui.GetCursorPosX();

            ImGui.Text("Cell Index");
            ImGui.SameLine();
            ImGui.SetCursorPosX(cursorX + 100);
            if (EditorGuiFieldsResolver.DrawIntField("#AtlasCurrentCellIndex", ref _chunkIndex))
            {
                _chunkIndex = Mathf.Clamp(_chunkIndex, 0, atlasData.ChunksCount - 1);
            }

            var currentCell = atlasData.GetCell(_chunkIndex);

            var position = new ivec2(currentCell.XPixel, currentCell.YPixel);

            ImGui.Text("Pixel Position");
            ImGui.SameLine();
            ImGui.SetCursorPosX(cursorX + 100);
            EditorGuiFieldsResolver.DrawVec2Field("#AtlasPixelPosition", ref position);
            currentCell.XPixel = Mathf.Clamp(position.x, 0, int.MaxValue);
            currentCell.YPixel = Mathf.Clamp(position.y, 0, int.MaxValue);

            var size = new ivec2(currentCell.Width, currentCell.Height);

            ImGui.Text("Size");
            ImGui.SameLine();
            ImGui.SetCursorPosX(cursorX + 100);
            EditorGuiFieldsResolver.DrawIVec2Field("#AtlasSize", ref size);
            currentCell.Width = Mathf.Clamp(size.x, 1, int.MaxValue);
            currentCell.Height = Mathf.Clamp(size.y, 1, int.MaxValue);

            vec2 pivotDim = new vec2(currentCell.Pivot.x, currentCell.Pivot.y);

            ImGui.Text("Pivot");
            ImGui.SameLine();
            ImGui.SetCursorPosX(cursorX + 100);
            EditorGuiFieldsResolver.DrawVec2Field("#AtlasPivotPos", ref pivotDim);
            currentCell.Pivot = new vec2(Mathf.Clamp(pivotDim.x, 0.0f, 1.0f), Mathf.Clamp(pivotDim.y, 0.0f, 1.0f));

            atlasData.UpdateChunk(_chunkIndex, currentCell);

            ImGui.EndDisabled();

            if (ImGui.Button("Apply All", new Vector2(ImGui.GetContentRegionAvail().X, 23)))
            {
                _meta.AtlasData = atlasData;
                EditorAssetUtils.WriteMeta(texture.Path, _meta);

                EditorAssetUtils.RefreshAssetDatabase();
            }
        }

        private TextureAtlasData GetAtlasData()
        {
            return _meta.AtlasData; //_text.Atlas; 
        }
    }
}
