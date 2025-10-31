using Engine.Graphics;
using Engine.Utils;
using FontStashSharp;
using FontStashSharp.Interfaces;
using GlmNet;
using System.Runtime.InteropServices;
using System.Text;

namespace Engine.GUI
{
    public enum TextOverflow { None, Clip, Ellipsis }
    public enum TextWrap { None, WordWrap }
    public enum TextFit { None, ShrinkToFit, ExpandToFit }

    public class UIText : UIGraphicsElement, IFontStashRenderer2
    {
        public FontAsset Font { get; set; }
        public Color Color { get; set; } = Color.White;
        public int FontSize { get; set; } = 32;
        public int MinFontSize { get; set; } = 8;
        public int MaxFontSize { get; set; } = 72;
        public float CharacterSpacing { get; set; }
        public float LineSpacing { get; set; }
        public int OutlineSize { get; set; }
        public TextVerticalAlignment Vertical { get => _verticalAlignment; set { if (_verticalAlignment != value) IsDirty = true; _verticalAlignment = value; } }
        public TextHorizontalAlignment Horizontal { get => _horizontalAlignment; set { if (_horizontalAlignment != value) IsDirty = true; _horizontalAlignment = value; } }
        public TextOverflow Overflow { get; set; } = TextOverflow.None;
        public TextWrap Wrap { get; set; } = TextWrap.None;
        public TextFit Fit { get; set; } = TextFit.None;
        private TextHorizontalAlignment _horizontalAlignment = TextHorizontalAlignment.Center;
        private TextVerticalAlignment _verticalAlignment = TextVerticalAlignment.Center;
        private readonly StringBuilder _text = new();
        public int Length => _text.Length;
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
            if (!Font)
                return;

            if (Fit != TextFit.None)
            {
                ApplyTextFit();
            }

            var font = FontManager.Instance.GetFont(Font).GetSpriteFont(FontSize);
            if (Wrap == TextWrap.WordWrap)
            {
                SendWrappedTextToDraw(font);
            }
            else
            {
                if (Overflow == TextOverflow.Ellipsis)
                {
                    string t = ApplyEllipsis(font);
                    SendTextToDraw(new StringBuilder(t), font, 0);
                }
                else
                {
                    SendTextToDraw(_text, font, 0);
                }
            }
        }

        private void ApplyTextFit()
        {
            var rect = RectTransform.Rect;
            var fontAsset = FontManager.Instance.GetFont(Font);
            if (Fit == TextFit.ShrinkToFit)
            {
                for (int size = FontSize; size >= MinFontSize; size--)
                {
                    var font = fontAsset.GetSpriteFont(size);
                    var b = font.TextBounds(_text, System.Numerics.Vector2.Zero);
                    if (b.X2 - b.X <= rect.Size.x && b.Y2 - b.Y <= rect.Size.y) { FontSize = size; break; }
                }
            }
            else if (Fit == TextFit.ExpandToFit)
            {
                for (int size = FontSize; size <= MaxFontSize; size++)
                {
                    var font = fontAsset.GetSpriteFont(size);
                    var b = font.TextBounds(_text, System.Numerics.Vector2.Zero);
                    if (b.X2 - b.X > rect.Size.x || b.Y2 - b.Y > rect.Size.y) { FontSize = Math.Max(MinFontSize, size - 1); break; }
                }
            }
        }

        private void SendWrappedTextToDraw(DynamicSpriteFont font)
        {
            float maxWidth = RectTransform.Rect.Size.x;
            string[] words = _text.ToString().Split(' ');
            StringBuilder line = new();
            StringBuilder finalText = new();
            foreach (var word in words)
            {
                string test = line.Length == 0 ? word : line + " " + word;
                var b = font.TextBounds(test, System.Numerics.Vector2.Zero);
                if (b.X2 - b.X > maxWidth)
                {
                    finalText.AppendLine(line.ToString());
                    line.Clear();
                    line.Append(word);
                }
                else 
                { 
                    line.Clear(); line.Append(test); 
                }
            }
            if (line.Length > 0)
            {
                finalText.Append(line);
            }

            SendTextToDraw(finalText, font, 0);
        }

        private string ApplyEllipsis(DynamicSpriteFont font)
        {
            var rect = RectTransform.Rect;
            string full = _text.ToString();
            for (int i = full.Length - 1; i > 0; i--)
            {
                string sub = full.Substring(0, i) + "...";
                var b = font.TextBounds(sub, System.Numerics.Vector2.Zero);
                if (b.X2 - b.X <= rect.Size.x)
                {
                    return sub;
                }
            }
            return full;
        }

        private void SendTextToDraw(StringBuilder text, DynamicSpriteFont font, int lineHeight)
        {
            if (IsDirty)
            {
                Mesh.IndicesToDrawCount = 0;
            }
            
            var rect = RectTransform.Rect;

            float rotation = glm.radians(Transform.WorldEulerAngles.z);
            var scale = new System.Numerics.Vector2(Transform.WorldScale.x, Transform.WorldScale.y);
            var effect = OutlineSize > 0 ? FontSystemEffect.Stroked : FontSystemEffect.None;

            var b = font.TextBounds(text, System.Numerics.Vector2.Zero, scale, CharacterSpacing, LineSpacing, effect, OutlineSize);
            float textWidth = b.X2 - b.X;
            float textHeight = b.Y2 - b.Y;

            System.Numerics.Vector2 pos = new(rect.Min.x, rect.Min.y);

            if (Horizontal == TextHorizontalAlignment.Center)
            {
                pos.X = rect.Min.x + (rect.Size.x - textWidth) * 0.5f;
            }
            else if (Horizontal == TextHorizontalAlignment.Right)
            {
                pos.X = rect.Min.x + rect.Size.x - textWidth;
            }

            if (Vertical == TextVerticalAlignment.Top)
            {
                pos.Y = rect.Min.y - b.Y;
            }
            else if (Vertical == TextVerticalAlignment.Center)
            {
                pos.Y = Mathf.Lerp(rect.YMax, rect.YMin, 0.5f) - textHeight;
            }
            else if (Vertical == TextVerticalAlignment.Bottom)
            {
                pos.Y = rect.Max.y - b.Y2;
            }

            pos.Y += lineHeight;

            font.DrawText(this, text, pos, new FSColor(Color), rotation, System.Numerics.Vector2.Zero, scale, 0,
                          CharacterSpacing, LineSpacing, TextStyle.None, effect, Math.Clamp(OutlineSize, 0, OutlineSize + 1));
        }

        public void SetText(string value) { _text.Clear(); AppendText(value); }
        public void Append(string value) { AppendText(value); }
        public void Append(char value) { AppendText(value); }
        public void Append(float value) { AppendText(value); }
        public void Append(int value) { AppendText(value); }
        private void AppendText<T>(T value) { IsDirty = true; _text.Append(value); }
    }
}
