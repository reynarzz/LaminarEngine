using Editor.AssemblyHotReload;
using Editor.Utils;
using Engine;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Views
{
    internal class TextureAtlasEditorView : EditorDrawerBase<Texture2D>
    {
        protected override bool AutoDrawTitle => false;

        private float _zoomFactor = 1.0f; // Default zoom level
        private const float _zoomSpeed = 0.1f; // Speed at which zooming occurs
        private const float _minZoom = 0.5f; // Minimum zoom factor (half size)
        private const float _maxZoom = 3.0f; // Maximum zoom factor (3x size)

        private int _chunkIndex = 0;

        private float _prevScrollAmount = -1;
        private vec2 _origin;
        private vec2 _cursorPos;
        private vec2 _cursorRelativePos;

        // Apply zoomFactor to the image size
        private vec2 _imageSize;
        private vec2 _zoomedSize;

        // Adjust image position based on zooming around the cursor position
        private vec2 _zoomedOrigin;
        private bool _isImageHovered = false;
        private ivec2 _sliceDim = new ivec2(8, 8);
        private bool _isOpen = true;

        internal override void OnOpen(Texture2D texture)
        {
            _chunkIndex = 0;
            _isOpen = true;
            _imageSize = new vec2(texture.Width, texture.Height);
            _sliceDim = new ivec2(8, 8);
            _zoomedSize = default;
            _origin = default;
            _prevScrollAmount = -1;
        }

        protected override void OnDraw(Texture2D texture)
        {
            //ImguiUtils::DisableNextWindowMenuButton();

            if (!_isOpen)
                return;

            ImGui.Begin("Atlas Editor", ref _isOpen);

            ImGui.BeginDisabled(GameAssemblyBuilder.IsBuilding /*|| project_builder.is_building_game()*/);

            var atlasInfo = texture.Atlas;

            // Handle zooming with mouse wheel
            float scrollAmount = ImGui.GetIO().MouseWheel;

            bool canMove = false;

            if (scrollAmount != _prevScrollAmount)
            {
                _prevScrollAmount = scrollAmount;
                canMove = true;
            }
            ImGui.Text(texture.Name);
            _origin = ImGui.GetCursorScreenPos().ToVec2();  // Get the starting position (top-left corner)

            if (canMove && ImGui.IsWindowHovered() && _isImageHovered) // remove
            {
                // Get the image's origin and cursor position relative to the image
                _cursorPos = ImGui.GetMousePos().ToVec2();      // Get the current mouse cursor position
                _cursorRelativePos = _cursorPos - _origin; // Cursor position relative to the image

                // Apply zoomFactor to the image size
                _zoomFactor += scrollAmount * _zoomSpeed;

                // Adjust image position based on zooming around the cursor position
                _zoomedOrigin = _cursorPos - _cursorRelativePos * _zoomFactor;
            }

            _zoomFactor = Mathf.Clamp(_zoomFactor, 0.1f, 5.0f); // Prevent zooming too far in or out


            _zoomedSize = new vec2(_imageSize.x * _zoomFactor, _imageSize.y * _zoomFactor);

            // Create a scrollable child for horizontal scrolling
            //--ImGui.SetCursorScreenPos(zoomedOrigin); // Set the image position based on zoom
            ImGui.SetCursorScreenPos(_origin.ToVector2()); // Set the image position based on zoom

            // Display the image
            ImGui.Image(EditorTextureDatabase.GetIconImGui(texture), _zoomedSize.ToVector2(), new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f));
            _isImageHovered = ImGui.IsItemHovered();

            // Display image size information
            ImGui.Text($"Size ({texture.Width}, {texture.Height})");

            // Scale grid drawing based on zoom factor
            //DrawGrid(zoomedOrigin, tex, zoomFactor);
            DrawGrid(_origin, texture, _zoomFactor);

            DrawProperties(texture);


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
                TextureAtlasUtils.SliceTiles(atlasInfo, _sliceDim.x, _sliceDim.y, texture.Width, texture.Height);
                _chunkIndex = 0;
            }

            // TODO: Feature to replace a cell index by another cell
            ImGui.Text("Replace cell index");
            ImGui.SameLine();
            int currentTest = 0;
            EditorGuiFieldsResolver.DrawIntField("#AtlasReplaceCellIndex", ref currentTest, 200);


            if (ImGui.Button("Save"))
            {
                // Updates every cell change. TODO: Only update the cells that changed.
                for (int i = 0; i < atlasInfo.ChunksCount; i++)
                {
                    var currentCell = atlasInfo.GetChunk(i);

                    atlasInfo.UpdateChunk(i, TextureAtlasUtils.CreateTileBounds(currentCell.XPixel, currentCell.YPixel, currentCell.Width, currentCell.Height,
                                                                                currentCell.Pivot.x, currentCell.Pivot.y, texture.Width, texture.Height));
                }

                // TODO: save
                var path = texture.Path;
                var texMeta = new TextureMetaFile();
                texMeta.GUID = texture.GetID();
                // texMeta.AtlasData = tex._textureInfo;

                Assets.RefreshAssetDatabase();
            }

            ImGui.EndDisabled();
            ImGui.End();
        }

        private void DrawGrid(vec2 origin, Texture2D tex, float zoom)
        {
            // Scale the grid cells based on zoom
            for (int i = 0; i < tex.Atlas.ChunksCount; i++)
            {
                if (_chunkIndex != i)
                {
                    DrawChunkLines(tex.Atlas.GetChunk(i), Color.Red);
                }
            }

            DrawChunkLines(tex.Atlas.GetChunk(_chunkIndex), Color.White);

            void DrawChunkLines(AtlasChunk chunk, Color color)
            {
                ImDrawListPtr draw_list = ImGui.GetWindowDrawList();

                float x1 = origin.x + chunk.XPixel * zoom;
                float y1 = origin.y + chunk.YPixel * zoom;
                float x2 = origin.x + (chunk.XPixel + chunk.Width) * zoom;
                float y2 = origin.y + (chunk.YPixel + chunk.Height) * zoom;

                // Draw the grid lines (quad outline)
                draw_list.AddLine(new Vector2(x1, y1), new Vector2(x1, y2), color); // Left
                draw_list.AddLine(new Vector2(x2, y1), new Vector2(x2, y2), color); // Right
                draw_list.AddLine(new Vector2(x1, y1), new Vector2(x2, y1), color); // Top
                draw_list.AddLine(new Vector2(x1, y2), new Vector2(x2, y2), color); // Bottom

                // Define the size of the smaller quad (scaled by zoom)
                //float smallWidth = (cell.width * 0.2f) * zoom;  // 20% of the original width
                //float smallHeight = (cell.height * 0.2f) * zoom; // 20% of the original height

                float smallWidth = 2;
                float smallHeight = 2;


                // Apply the pivot using lerp between the corners (pivotX and pivotY as percentage)
                float lerpX = Mathf.Lerp(x1, x2, chunk.Pivot.x);
                float lerpY = Mathf.Lerp(y1, y2, chunk.Pivot.y);

                // Calculate the corners of the small quad, centered around the pivot
                float smallX1 = lerpX - smallWidth / 2.0f;
                float smallY1 = lerpY - smallHeight / 2.0f;
                float smallX2 = lerpX + smallWidth / 2.0f;
                float smallY2 = lerpY + smallHeight / 2.0f;

                // Draw the small filled quad at the pivot position
                draw_list.AddRectFilled(new Vector2(smallX1, smallY1), new Vector2(smallX2, smallY2), Color.White); // Filled quad
            }
        }

        private void DrawProperties(Texture2D texture)
        {
            var atlasInfo = texture.Atlas;

            if (atlasInfo.ChunksCount > 0)
            {
                float cursorX = ImGui.GetCursorPosX();

                ImGui.Text("Cell Index");
                ImGui.SameLine();
                ImGui.SetCursorPosX(cursorX + 100);
                if (EditorGuiFieldsResolver.DrawIntField("#AtlasCurrentCellIndex", ref _chunkIndex))
                {
                    _chunkIndex = Mathf.Clamp(_chunkIndex, 0, atlasInfo.ChunksCount - 1);
                }

                var currentCell = atlasInfo.GetChunk(_chunkIndex);

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
                currentCell.Width = (int)Mathf.Clamp(size.x, 1, int.MaxValue);
                currentCell.Height = (int)Mathf.Clamp(size.y, 1, int.MaxValue);

                vec2 pivotDim = new vec2(currentCell.Pivot.x, currentCell.Pivot.y);

                ImGui.Text("Pivot");
                ImGui.SameLine();
                ImGui.SetCursorPosX(cursorX + 100);
                EditorGuiFieldsResolver.DrawVec2Field("#AtlasPivotPos", ref pivotDim);
                currentCell.Pivot = new vec2(Mathf.Clamp(pivotDim.x, 0.0f, 1.0f), Mathf.Clamp(pivotDim.y, 0.0f, 1.0f));

                atlasInfo.UpdateChunk(_chunkIndex, currentCell);
            }
        }
    }
}
