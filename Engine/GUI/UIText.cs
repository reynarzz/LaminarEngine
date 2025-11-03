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

    public struct Thickness
    {
        public float Left, Top, Right, Bottom;
        public Thickness(float uniform) { Left = Top = Right = Bottom = uniform; }
        public Thickness(float left, float top, float right, float bottom) { Left = left; Top = top; Right = right; Bottom = bottom; }
    }

    public class UIText : UIGraphicsElement, IFontStashRenderer2
    {
        public FontAsset Font { get; set; }
        public int FontResolution { get; set; } = 1;
        public int FontSize { get; set; } = 32;
        public int MinFontSize { get; set; } = 8;
        public int MaxFontSize { get; set; } = 72;
        public float CharacterSpacing { get; set; }
        public float LineSpacing { get; set; }
        public int OutlineSize { get; set; }
        public Thickness Padding = new Thickness(0);
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
        private uint ARGBtoRGBA(uint packed)
        {
            byte r = (byte)(packed);
            byte g = (byte)(packed >> 8);
            byte b = (byte)(packed >> 16);
            byte a = (byte)(packed >> 24);
            return (uint)(r << 24 | g << 16 | b << 8 | a);
        }

        private void SetFontVertex(Span<Vertex> fVertex, ref VertexPositionColorTexture vertex, int vertexIndex)
        {
            fVertex[vertexIndex].Position = new vec2(vertex.Position.X, vertex.Position.Y);
            fVertex[vertexIndex].UV = new vec2(vertex.TextureCoordinate.X, vertex.TextureCoordinate.Y);
            fVertex[vertexIndex].Color = ARGBtoRGBA(vertex.Color.PackedValue);
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

            var font = FontManager.Instance.GetFont(Font).GetSpriteFont(FontResolution, FontSize);
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
            var padded = GetPaddedRect();
            var fontAsset = FontManager.Instance.GetFont(Font);
            if (Fit == TextFit.ShrinkToFit)
            {
                for (int size = FontSize; size >= MinFontSize; size--)
                {
                    var font = fontAsset.GetSpriteFont(FontResolution, size);
                    var b = font.TextBounds(_text, System.Numerics.Vector2.Zero);
                    if (b.X2 - b.X <= padded.z - padded.x && b.Y2 - b.Y <= padded.w - padded.y) { FontSize = size; break; }
                }
            }
            else if (Fit == TextFit.ExpandToFit)
            {
                for (int size = FontSize; size <= MaxFontSize; size++)
                {
                    var font = fontAsset.GetSpriteFont(FontResolution, size);
                    var b = font.TextBounds(_text, System.Numerics.Vector2.Zero);
                    if (b.X2 - b.X > padded.z - padded.x || b.Y2 - b.Y > padded.w - padded.y) { FontSize = Math.Max(MinFontSize, size - 1); break; }
                }
            }
        }

        private void SendWrappedTextToDraw(DynamicSpriteFont font)
        {
            var padded = GetPaddedRect();
            float maxWidth = padded.z - padded.x;
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
            if (line.Length > 0) finalText.Append(line);
            SendTextToDraw(finalText, font, 0);
        }

        private string ApplyEllipsis(DynamicSpriteFont font)
        {
            var padded = GetPaddedRect();
            string full = _text.ToString();
            for (int i = full.Length - 1; i > 0; i--)
            {
                string sub = full.Substring(0, i) + "...";
                var b = font.TextBounds(sub, System.Numerics.Vector2.Zero);
                if (b.X2 - b.X <= padded.z - padded.x)
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

            var padded = GetPaddedRect();
            float rotation = glm.radians(Transform.WorldEulerAngles.z);
            var scale = new System.Numerics.Vector2(Transform.WorldScale.x, Transform.WorldScale.y);
            var effect = OutlineSize > 0 ? FontSystemEffect.Stroked : FontSystemEffect.None;

            var b = font.TextBounds(text, System.Numerics.Vector2.Zero, scale, CharacterSpacing, LineSpacing, effect, OutlineSize);
            float textWidth = b.X2 - b.X;
            float textHeight = b.Y2 - b.Y;
            ResizeRectToText(textWidth, textHeight);

            System.Numerics.Vector2 pos = new(padded.x, padded.y);

            if (Horizontal == TextHorizontalAlignment.Center)
                pos.X = padded.x + ((padded.z - padded.x) - textWidth) * 0.5f;
            else if (Horizontal == TextHorizontalAlignment.Right)
                pos.X = padded.z - textWidth;

            if (Vertical == TextVerticalAlignment.Top)
                pos.Y = padded.y - b.Y;
            else if (Vertical == TextVerticalAlignment.Center)
                pos.Y = (padded.w - b.Y2 + padded.y - b.Y) * 0.5f;
            else if (Vertical == TextVerticalAlignment.Bottom)
                pos.Y = padded.w - b.Y2;

            pos.Y += lineHeight;

            font.DrawText(this, text, pos, new FSColor(Color.R, Color.G, Color.B, Color.A), rotation, System.Numerics.Vector2.Zero, scale, 0,
                          CharacterSpacing, LineSpacing, TextStyle.None, effect, Math.Clamp(OutlineSize, 0, OutlineSize + 1));


        }
        private void ResizeRectToText(float textWidth, float textHeight)
        {
            if (Fit == TextFit.ExpandToFit)
            {
                var size = RectTransform.Size;
                size.x = textWidth + Padding.Left + Padding.Right;
                size.y = textHeight + Padding.Top + Padding.Bottom;
                RectTransform.Size = size;
                RectTransform.Recalculate(RectTransform.Parent);
            }
        }
        private vec4 GetPaddedRect()
        {
            var r = RectTransform.Rect;
            return new vec4(r.Min.x + Padding.Left,
                            r.Min.y + Padding.Top,
                            r.Max.x - Padding.Right,
                            r.Max.y - Padding.Bottom
            );
        }

        public void SetText(string value) { _text.Clear(); AppendText(value); }
        public void Append(string value) { AppendText(value); }
        public void Append(char value) { AppendText(value); }
        public void Append(float value) { AppendText(value); }
        public void Append(int value) { AppendText(value); }
        private void AppendText<T>(T value) { IsDirty = true; _text.Append(value); }
    }
}
