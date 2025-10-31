using GlmNet;

namespace Engine.GUI
{
    public class UIImage : UIGraphicsElement
    {
        public bool PreserveAspect { get; set; }

        internal override void OnInitialize()
        {
            base.OnInitialize();
            Mesh = new Mesh();
            Mesh.Vertices.Add(default); Mesh.Vertices.Add(default);
            Mesh.Vertices.Add(default); Mesh.Vertices.Add(default);
            Mesh.IndicesToDrawCount = 6;
        }

        internal override void OnCanvasDraw(UICanvas canvas)
        {
            var rt = RectTransform;
            var size = rt.ComputedRect.Size;
            if (size.x <= 0f || size.y <= 0f) return;

            var chunk = Sprite?.GetAtlasChunk() ?? AtlasChunk.DefaultChunk;

            if (PreserveAspect && chunk.Width > 0 && chunk.Height > 0)
            {
                float spriteRatio = (float)chunk.Width / (float)chunk.Height;
                float rectRatio = size.x / size.y;

                if (spriteRatio > rectRatio)
                {
                    float newH = size.x / spriteRatio;
                    size = new vec2(size.x, newH);
                }
                else
                {
                    float newW = size.y * spriteRatio;
                    size = new vec2(newW, size.y);
                }
            }

            var quad = GraphicsHelper.GetUIQuadVerticesLocal(chunk.Uvs, size, rt.Pivot, Color, Transform.WorldMatrix);

            Mesh.Vertices[0] = quad.v0;
            Mesh.Vertices[1] = quad.v1;
            Mesh.Vertices[2] = quad.v2;
            Mesh.Vertices[3] = quad.v3;

            // IsDirty = true;
        }
    }
}
