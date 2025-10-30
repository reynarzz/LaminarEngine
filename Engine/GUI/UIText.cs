using Engine.Graphics;
using Engine.Utils;
using FontStashSharp;
using FontStashSharp.Interfaces;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public class UIText : UIElement, IFontStashRenderer2
    {
        public FontAsset Font { get; set; }

        public Color Color = Color.White;
        public int FontSize { get; set; } = 32;
        public float CharacterSpacing { get; set; }
        public float LineSpacing { get; set; }
        public int OutlineSize { get; set; }

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

        private Vertex[] _vertexData;
        private TextVerticalAlignment _verticalAlignment = TextVerticalAlignment.Top;
        private TextHorizontalAlignment _horizontalAlignment = TextHorizontalAlignment.Left;
        private int _vertexIndex = 0;
        private readonly StringBuilder _text = new();

        ITexture2DManager IFontStashRenderer2.TextureManager => FontManager.Instance.TextureManager;

        public void SetText(string text)
        {
            _text.Clear();
            _text.Append(text);
        }
        public void AddText(string text)
        {
            _text.Append(text);
        }

        internal override void OnInitialize()
        {
            base.OnInitialize();

            _vertexIndex = 0;
            _vertexData = new Vertex[Consts.Graphics.MAX_QUADS_PER_BATCH * 4];
            Mesh.Vertices.Capacity = Consts.Graphics.MAX_QUADS_PER_BATCH * 4;

        }

        public void DrawQuad(object texture, ref VertexPositionColorTexture topLeft,
                                             ref VertexPositionColorTexture topRight,
                                             ref VertexPositionColorTexture bottomLeft,
                                             ref VertexPositionColorTexture bottomRight)
        {
            var tex = texture as Texture2D;
            Material.AddTexture(tex.Name, tex);


            // TODO: send texture unit to vertices.
            if (_vertexData.Length > _vertexIndex + 4)
            {
                SetFontVertex(_vertexData, ref bottomLeft, _vertexIndex + 0);
                SetFontVertex(_vertexData, ref topLeft, _vertexIndex + 1);
                SetFontVertex(_vertexData, ref topRight, _vertexIndex + 2);
                SetFontVertex(_vertexData, ref bottomRight, _vertexIndex + 3);

                _vertexIndex += 4;
            }
        }

        private void SetFontVertex(Vertex[] fVertex, ref VertexPositionColorTexture vertex, int vertexIndex)
        {
            fVertex[vertexIndex].Position = new vec2(vertex.Position.X, vertex.Position.Y);
            fVertex[vertexIndex].UV = new vec2(vertex.TextureCoordinate.X, vertex.TextureCoordinate.Y);
            fVertex[vertexIndex].Color = vertex.Color.PackedValue;
            fVertex[vertexIndex].VertexIndex = vertexIndex;
        }

        internal override void Draw()
        {
            SendTextToDraw(_text, FontManager.Instance.GetFont(Font).GetSpriteFont(FontSize), 0);
        }

        private void SendTextToDraw(StringBuilder text, DynamicSpriteFont font, int lineHeight)
        {
            _vertexIndex = 0;

            var pivot = new System.Numerics.Vector2(0.0f, 1.0f);

            switch (Horizontal)
            {
                case TextHorizontalAlignment.Center:
                    pivot.X = 0.5f;
                    break;
                case TextHorizontalAlignment.Left:
                    pivot.X = 0.0f;
                    break;
                case TextHorizontalAlignment.Right:
                    pivot.X = 1.0f;
                    break;
            }

            switch (Vertical)
            {
                case TextVerticalAlignment.Center:
                    pivot.Y = 0.5f;
                    break;
                case TextVerticalAlignment.Bottom:
                    pivot.Y = 1.0f;
                    break;
                case TextVerticalAlignment.Top:
                    pivot.Y = 0.0f;
                    break;
            }

            float rotation = glm.radians(Transform.WorldEulerAngles.z);

            var scale = new System.Numerics.Vector2(Transform.WorldScale.x,
                                                    Transform.WorldScale.y);

            var effect = OutlineSize > 0 ? FontSystemEffect.Stroked : FontSystemEffect.None;

            //var size = font.MeasureString(text, scale, CharacterSpacing, LineSpacing, effect, OutlineSize);
            //var origin = new System.Numerics.Vector2(size.X, size.Y);
            var origin = default(System.Numerics.Vector2);
            var position = new System.Numerics.Vector2(Transform.WorldPosition.x,
                                                       Transform.WorldPosition.y + lineHeight);

            var bounds = font.TextBounds(text, position, scale, CharacterSpacing, LineSpacing, effect, OutlineSize);

            var textSize = new System.Numerics.Vector2(bounds.X2 - bounds.X, bounds.Y2 - bounds.Y);

            // Compute pivot offset
            var pivotOffset = new System.Numerics.Vector2(textSize.X * pivot.X, textSize.Y * pivot.Y);

            // Final position to start rendering from
            var finalPosition = position - pivotOffset;

            // This line calls the DrawQuad function for every character.
            font.DrawText(this, text, finalPosition, new FSColor(Color), rotation, origin, scale, 0,
                          CharacterSpacing, LineSpacing, TextStyle.None,
                          effect, Math.Clamp(OutlineSize, 0, OutlineSize + 1));
        }
    }
}
