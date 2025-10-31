using GlmNet;

namespace Engine.GUI
{
    public enum PreserveAspectMode
    {
        MatchWidth,
        MatchHeight
    }

    public class UIImage : UIGraphicsElement
    {
        public bool PreserveAspect { get; set; } = false;
        public PreserveAspectMode AspectMode { get; set; } = PreserveAspectMode.MatchWidth;

        internal override void OnInitialize()
        {
            base.OnInitialize();

            Mesh = new Mesh();
            Mesh.Vertices.Add(default);
            Mesh.Vertices.Add(default);
            Mesh.Vertices.Add(default);
            Mesh.Vertices.Add(default);
            Mesh.IndicesToDrawCount = 6;
        }

        internal override void OnCanvasDraw(UICanvas canvas)
        {
            var rect = RectTransform.ComputedRect;
            var size = rect.Size;

            // Adjust size for aspect ratio
            if (PreserveAspect && Sprite?.Texture != null)
            {
                float texW = Sprite.Texture.Width;
                float texH = Sprite.Texture.Height;

                if (texH > 0.0f)
                {
                    float textureRatio = texW / texH;
                    float rectW = size.x;
                    float rectH = size.y;
                    float rectRatio = rectW / rectH;

                    if (AspectMode == PreserveAspectMode.MatchHeight)
                    {
                        float newWidth = rectH * textureRatio;
                        float delta = (rectW - newWidth) * 0.5f;
                        rect = new Rect(
                            new vec2(rect.X + delta, rect.Y),
                            new vec2(newWidth, rectH)
                        );
                    }
                    else // MatchWidth
                    {
                        float newHeight = rectW / textureRatio;
                        float delta = (rectH - newHeight) * 0.5f;
                        rect = new Rect(
                            new vec2(rect.X, rect.Y + delta),
                            new vec2(rectW, newHeight)
                        );
                    }
                }
            }

            var verts = GraphicsHelper.GetUIQuadVertices(rect,  Color);

            Mesh.Vertices[0] = verts.v0;
            Mesh.Vertices[1] = verts.v1;
            Mesh.Vertices[2] = verts.v2;
            Mesh.Vertices[3] = verts.v3;

            IsDirty = true;
        }
    }
}
