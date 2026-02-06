using GlmNet;
using Engine.GUI;
using System.Runtime.CompilerServices;
using Engine.Graphics;
using Engine;

namespace Engine.GUI
{
    public class UIImage : UIGraphicsElement
    {
        private bool _preserveAspect = false;
        public bool PreserveAspect
        {
            get => _preserveAspect;
            set
            {
                if (_preserveAspect == value)
                    return;

                RendererData.IsDirty = true;

                _preserveAspect = value;
            }
        }

        private bool _isSliced = false;
        public bool IsSliced
        {
            get => _isSliced;
            set
            {
                if (_isSliced == value)
                    return;

                RendererData.IsDirty = true;

                _isSliced = value;
            }
        }

        // TODO: set image dirty when any property changes.

        // Slice sizes in pixels
        public float LeftBorder = 10f;
        public float RightBorder = 10f;
        public float BottomBorder = 10f;
        public float TopBorder = 10f;

        private int _currentSliceVertexIndex = 0;
        private RendererData2D _rendererData;

        public float SlicedBorderResolution { get; set; } = 1;
        protected override void OnAwake()
        {
            base.OnAwake();

            _rendererData = (RendererData as RendererData2D);
            _rendererData.Mesh = new Mesh();

            _rendererData.Mesh.IndicesToDrawCount = 6;
            for (int i = 0; i < 4; i++)
            {
                _rendererData.Mesh.Vertices.Add(default);
            }
        }

        internal override void OnCanvasDraw(UICanvas canvas)
        {
            var rt = RectTransform;
            var size = rt.Rect.Size;
            if (size.x <= 0f || size.y <= 0f)
            {
                Debug.Log("Zero size image: " + Name);
                return;
            }

            if (RendererData.IsDirty)
            {
                if (!IsSliced)
                {
                    DrawSimpleQuad(rt, size);
                }
                else
                {
                    Draw9SliceQuad(rt, size);
                }
            }
        }

        private void DrawSimpleQuad(RectTransform rt, vec2 size)
        {
            var chunk = Sprite?.GetAtlasCell() ?? TextureAtlasCell.DefaultChunk;
            _rendererData.Mesh.IndicesToDrawCount = 6;

            if (PreserveAspect && chunk.Width > 0 && chunk.Height > 0)
            {
                float spriteRatio = (float)chunk.Width / chunk.Height;
                float rectRatio = size.x / size.y;

                if (spriteRatio > rectRatio)
                    size.y = size.x / spriteRatio;
                else
                    size.x = size.y * spriteRatio;
            }

            var quad = GraphicsHelper.GetUIQuadVerticesLocal(chunk.Uvs, size, rt.Pivot, Color, Transform.WorldMatrix);

            _rendererData.Mesh.Vertices[0] = quad.v0;
            _rendererData.Mesh.Vertices[1] = quad.v1;
            _rendererData.Mesh.Vertices[2] = quad.v2;
            _rendererData.Mesh.Vertices[3] = quad.v3;
        }

        private void Draw9SliceQuad(RectTransform rt, vec2 size)
        {
            var chunk = Sprite?.GetAtlasCell() ?? TextureAtlasCell.DefaultChunk;

            var uv = QuadUV.FlipUV(chunk.Uvs, false, true);

            float width = chunk.Width * SlicedBorderResolution;
            float height = chunk.Height * SlicedBorderResolution;

            if (PreserveAspect && width > 0 && height > 0)
            {
                float spriteRatio = (float)width / height;
                float rectRatio = size.x / size.y;
                if (spriteRatio > rectRatio) size.y = size.x / spriteRatio;
                else size.x = size.y * spriteRatio;
            }

            float left = LeftBorder, right = RightBorder, bottom = BottomBorder, top = TopBorder;

            float x0 = -rt.Pivot.x * size.x;
            float y0 = -rt.Pivot.y * size.y;
            float x1 = x0 + left;
            float x2 = x0 + size.x - right;
            float x3 = x0 + size.x;
            float y1 = y0 + top;
            float y2 = y0 + size.y - bottom;
            float y3 = y0 + size.y;

            float uMin = MathF.Min(MathF.Min(uv.BottomLeftUV.x, uv.TopLeftUV.x), MathF.Min(uv.BottomRightUV.x, uv.TopRightUV.x));
            float uMax = MathF.Max(MathF.Max(uv.BottomLeftUV.x, uv.TopLeftUV.x), MathF.Max(uv.BottomRightUV.x, uv.TopRightUV.x));
            float vMin = MathF.Min(MathF.Min(uv.BottomLeftUV.y, uv.TopLeftUV.y), MathF.Min(uv.BottomRightUV.y, uv.TopRightUV.y));
            float vMax = MathF.Max(MathF.Max(uv.BottomLeftUV.y, uv.TopLeftUV.y), MathF.Max(uv.BottomRightUV.y, uv.TopRightUV.y));

            float u0 = uMin;
            float u1 = uMin + (left / width) * (uMax - uMin);
            float u2 = uMax - (right / height) * (uMax - uMin);
            float u3 = uMax;

            float v0 = vMax;
            float v1 = vMax - (top / width) * (vMax - vMin);
            float v2 = vMin + (bottom / height) * (vMax - vMin);
            float v3 = vMin;

            _currentSliceVertexIndex = 0;
            _rendererData.Mesh.IndicesToDrawCount = 0;

            for (int iy = 0; iy < 3; iy++)
            {
                float yA = iy == 0 ? y0 : (iy == 1 ? y1 : y2);
                float yB = iy == 0 ? y1 : (iy == 1 ? y2 : y3);
                float vA = iy == 0 ? v0 : (iy == 1 ? v1 : v2);
                float vB = iy == 0 ? v1 : (iy == 1 ? v2 : v3);

                for (int ix = 0; ix < 3; ix++)
                {
                    float xA = ix == 0 ? x0 : (ix == 1 ? x1 : x2);
                    float xB = ix == 0 ? x1 : (ix == 1 ? x2 : x3);
                    float uA = ix == 0 ? u0 : (ix == 1 ? u1 : u2);
                    float uB = ix == 0 ? u1 : (ix == 1 ? u2 : u3);

                    _rendererData.Mesh.IndicesToDrawCount += 6;
                    AddVertex(xA, yA, uA, vA);
                    AddVertex(xA, yB, uA, vB);
                    AddVertex(xB, yB, uB, vB);
                    AddVertex(xB, yA, uB, vA);
                }
            }

            void AddVertex(float x, float y, float u, float v)
            {
                vec4 wp = Transform.WorldMatrix * new vec4(x, y, 0, 1);

                if (_rendererData.Mesh.Vertices.Count <= _currentSliceVertexIndex)
                {
                    _rendererData.Mesh.Vertices.Add(default);
                }

                _rendererData.Mesh.Vertices[_currentSliceVertexIndex++] = new Vertex()
                {
                    Position = new vec2(wp.x, wp.y),
                    UV = new vec2(u, v),
                    Color = Color
                };
            }

#if DEBUG
            if (Debug.DrawUILines)
            {
                DebugDrawSlices(x0, x1, x2, x3, y0, y1, y2, y3);
            }
#endif
        }

        private void DebugDrawSlices(float x0, float x1, float x2, float x3,
                                     float y0, float y1, float y2, float y3)
        {

            for (int iy = 0; iy < 3; iy++)
            {
                float yA = iy == 0 ? y0 : (iy == 1 ? y1 : y2);
                float yB = iy == 0 ? y1 : (iy == 1 ? y2 : y3);

                for (int ix = 0; ix < 3; ix++)
                {
                    float xA = ix == 0 ? x0 : (ix == 1 ? x1 : x2);
                    float xB = ix == 0 ? x1 : (ix == 1 ? x2 : x3);

                    var center = Transform.WorldMatrix * new vec4((xA + xB) * 0.5f,
                                                                   (yA + yB) * 0.5f, 0, 1);

                    var size = new vec3(xB - xA, yB - yA, 0);

                    Debug.DrawBoxUI(new vec3(center.x, center.y, 0), size, Color.Cyan);
                }
            }
        }

    }
}
