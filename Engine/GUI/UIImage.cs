using GlmNet;
using Engine.GUI;
using System.Runtime.CompilerServices;

namespace Engine.GUI
{
    public class UIImage : UIGraphicsElement
    {
        public bool PreserveAspect { get; set; } = false;

        public bool IsSliced { get; set; } = false;

        // Slice sizes in pixels
        public float LeftBorder = 10f;
        public float RightBorder = 10f;
        public float TopBorder = 10f;
        public float BottomBorder = 10f;

        private int _currentSliceVertexIndex = 0;

        internal override void OnInitialize()
        {
            base.OnInitialize();
            Mesh = new Mesh();
            Mesh.IndicesToDrawCount = 6;
            for (int i = 0; i < 4; i++)
            {
                Mesh.Vertices.Add(default);
            }
        }

        internal override void OnCanvasDraw(UICanvas canvas)
        {
            var rt = RectTransform;
            var size = rt.Rect.Size;
            if (size.x <= 0f || size.y <= 0f) return;

            if (!IsSliced)
            {
                DrawSimpleQuad(rt, size);
            }
            else
            {
                Draw9SliceQuad(rt, size);
            }
        }

        private void DrawSimpleQuad(RectTransform rt, vec2 size)
        {
            var chunk = Sprite?.GetAtlasChunk() ?? AtlasChunk.DefaultChunk;
            Mesh.IndicesToDrawCount = 6;

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

            Mesh.Vertices[0] = quad.v0;
            Mesh.Vertices[1] = quad.v1;
            Mesh.Vertices[2] = quad.v2;
            Mesh.Vertices[3] = quad.v3;
        }

        private void Draw9SliceQuad(RectTransform rt, vec2 size)
        {
            var chunk = Sprite?.GetAtlasChunk() ?? AtlasChunk.DefaultChunk;
            var uv = QuadUV.FlipUV(chunk.Uvs, false, true);

            Mesh.IndicesToDrawCount = 54;

            if (PreserveAspect && chunk.Width > 0 && chunk.Height > 0)
            {
                float spriteRatio = (float)chunk.Width / chunk.Height;
                float rectRatio = size.x / size.y;

                if (spriteRatio > rectRatio)
                {
                    size.y = size.x / spriteRatio;
                }
                else
                {
                    size.x = size.y * spriteRatio;
                }
            }

            float l = LeftBorder, r = RightBorder, t = TopBorder, b = BottomBorder;

            float x0 = -rt.Pivot.x * size.x;
            float y0 = -rt.Pivot.y * size.y;

            float x1 = x0 + l;
            float x2 = x0 + size.x - r;
            float x3 = x0 + size.x;

            float y1 = y0 + b;
            float y2 = y0 + size.y - t;
            float y3 = y0 + size.y;

            float u0 = uv.BottomLeftUV.x;
            float u1 = uv.BottomLeftUV.x + l / chunk.Width;
            float u2 = uv.TopRightUV.x - r / chunk.Width;
            float u3 = uv.TopRightUV.x;

            float v0 = uv.BottomLeftUV.y;
            float fullHeight = uv.TopRightUV.y - uv.BottomLeftUV.y;
            float v1 = uv.BottomLeftUV.y + (b / chunk.Height) * fullHeight;
            float v2 = uv.TopRightUV.y - (t / chunk.Height) * fullHeight;
            float v3 = uv.TopRightUV.y;

            _currentSliceVertexIndex = 0;

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

                    AddVertex(xA, yA, uA, vA);
                    AddVertex(xA, yB, uA, vB);
                    AddVertex(xB, yB, uB, vB);
                    AddVertex(xB, yA, uB, vA);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddVertex(float x, float y, float u, float v)
        {
            vec4 wp = Transform.WorldMatrix * new vec4(x, y, 0, 1);

            if (Mesh.Vertices.Count <= _currentSliceVertexIndex)
            {
                Mesh.Vertices.Add(default);
            }

            Mesh.Vertices[_currentSliceVertexIndex++] = new Vertex()
            {
                Position = new vec2(wp.x, wp.y),
                UV = new vec2(u, v),
                Color = Color
            };
        }
    }
}
