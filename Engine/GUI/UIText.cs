using Engine.Graphics;
using Engine.Utils;
using FontStashSharp;
using FontStashSharp.Interfaces;
using GlmNet;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.GUI
{
    public class UIText : UIGraphicsElement, IFontStashRenderer2
    {
        public FontAsset Font { get; set; }
        public Color Color { get; set; } = Color.White;
        public int FontSize { get; set; } = 32;
        public float CharacterSpacing { get; set; }
        public float LineSpacing { get; set; }
        public int OutlineSize { get; set; }
        public int Length => _text.Length;
        public TextVerticalAlignment Vertical
        {
            get => _verticalAlignment;
            set
            {
                if (_verticalAlignment != value)
                {
                    IsDirty = true;
                }
                _verticalAlignment = value;

            }
        }

        public TextHorizontalAlignment Horizontal
        {
            get => _horizontalAlignment;
            set
            {
                if (_horizontalAlignment != value)
                {
                    IsDirty = true;
                }
                _horizontalAlignment = value;
            }
        }

        private TextHorizontalAlignment _horizontalAlignment = TextHorizontalAlignment.Center;
        private TextVerticalAlignment _verticalAlignment = TextVerticalAlignment.Center;
        private readonly StringBuilder _text = new();

        ITexture2DManager IFontStashRenderer2.TextureManager => FontManager.Instance.TextureManager;

        internal override void OnInitialize()
        {
            base.OnInitialize();

            Mesh = new Mesh();
            Mesh.Vertices.Capacity = Consts.Graphics.MAX_QUADS_PER_BATCH * 4;
            Sprite = new Sprite();
        }

        public void DrawQuad(object texture, ref VertexPositionColorTexture topLeft,
                                             ref VertexPositionColorTexture topRight,
                                             ref VertexPositionColorTexture bottomLeft,
                                             ref VertexPositionColorTexture bottomRight)
        {
            if (IsDirty)
            {
                Sprite.Texture = texture as Texture2D;
                var vertIndex = (Mesh.IndicesToDrawCount / 6) * 4;

                if (Mesh.Vertices.Count < vertIndex + 4)
                {
                    Mesh.Vertices.Add(default);
                    Mesh.Vertices.Add(default);
                    Mesh.Vertices.Add(default);
                    Mesh.Vertices.Add(default);
                }

                var verts = CollectionsMarshal.AsSpan(Mesh.Vertices);

                SetFontVertex(verts, ref bottomLeft, vertIndex + 0);
                SetFontVertex(verts, ref topLeft, vertIndex + 1);
                SetFontVertex(verts, ref topRight, vertIndex + 2);
                SetFontVertex(verts, ref bottomRight, vertIndex + 3);

                Mesh.IndicesToDrawCount += 6;
            }
        }

        private void SetFontVertex(Span<Vertex> fVertex, ref VertexPositionColorTexture vertex, int vertexIndex)
        {
            fVertex[vertexIndex].Position = new vec2(vertex.Position.X, vertex.Position.Y);
            fVertex[vertexIndex].UV = new vec2(vertex.TextureCoordinate.X, vertex.TextureCoordinate.Y);
            fVertex[vertexIndex].Color = vertex.Color.PackedValue;
            fVertex[vertexIndex].VertexIndex = vertexIndex;
        }

        internal override void OnCanvasDraw(UICanvas canvas)
        {
            if (Font)
            {
                FontSize = Math.Clamp(FontSize, 1, 1000);
                SendTextToDraw(_text, FontManager.Instance.GetFont(Font).GetSpriteFont(FontSize), 0);
            }
            else
            {
                Debug.Warn($"Font for text '{Name}' is not set");
            }
        }

        private void SendTextToDraw(StringBuilder text, DynamicSpriteFont font, int lineHeight)
        {
            if (IsDirty)
                Mesh.IndicesToDrawCount = 0;

            var rect = RectTransform.Rect;
            float rotation = glm.radians(Transform.WorldEulerAngles.z);
            var scale = new System.Numerics.Vector2(Transform.WorldScale.x, Transform.WorldScale.y);
            var effect = OutlineSize > 0 ? FontSystemEffect.Stroked : FontSystemEffect.None;

            var b = font.TextBounds(text, System.Numerics.Vector2.Zero, scale, CharacterSpacing, LineSpacing, effect, OutlineSize);
            float textWidth = b.X2 - b.X;
            float textHeight = b.Y2 - b.Y;

            System.Numerics.Vector2 finalPos = new System.Numerics.Vector2(rect.Min.x, rect.Min.y);

            switch (Horizontal)
            {
                case TextHorizontalAlignment.Left:
                    finalPos.X = rect.Min.x;
                    break;
                case TextHorizontalAlignment.Center:
                    finalPos.X = rect.Min.x + (rect.Size.x - textWidth) * 0.5f;
                    break;
                case TextHorizontalAlignment.Right:
                    finalPos.X = rect.Min.x + rect.Size.x - textWidth;
                    break;
            }

            switch (Vertical)
            {
                case TextVerticalAlignment.Top:
                    finalPos.Y = rect.Min.y - b.Y;
                    break;
                case TextVerticalAlignment.Center:
                    finalPos.Y = Mathf.Lerp(rect.YMax, rect.YMin, 0.5f) - textHeight;
                    break;
                case TextVerticalAlignment.Bottom:
                    finalPos.Y = rect.Max.y - b.Y2;
                    break;
            }

            finalPos.Y += lineHeight;

            font.DrawText(this, text, finalPos, new FSColor(Color),
                rotation, System.Numerics.Vector2.Zero, scale, 0,
                CharacterSpacing, LineSpacing, TextStyle.None,
                effect, Math.Clamp(OutlineSize, 0, OutlineSize + 1));
        }

        public void SetText(string value)
        {
            _text.Clear();
            AppendText(value);
        }
        public void Append(string value)
        {
            AppendText(value);
        }
        public void Append(char value)
        {
            AppendText(value);
        }
        public void Append(float value)
        {
            AppendText(value);
        }
        public void Append(int value)
        {
            AppendText(value);
        }

        private void AppendText<T>(T value)
        {
            IsDirty = true;
            _text.Append(value);
        }

    }
}