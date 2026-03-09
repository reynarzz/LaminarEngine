using Engine.Utils;
using FontStashSharp;
using GlmNet;
using System.Runtime.InteropServices;

namespace Engine.Graphics
{
    internal class FontManager
    {
        private static FontManager _instance;
        public static FontManager Instance => _instance ?? (_instance = new FontManager());

        private static FontTextureManager _textureManager;
        public FontTextureManager TextureManager => _textureManager;

        private readonly Dictionary<string, FontContent> _fontFamilies;
        internal class FontContent
        {
            private class FontSystemContainer
            {
                private readonly FontSystem _fontSystem;
                private readonly Dictionary<int, DynamicSpriteFont> _spriteFonts;

                public FontSystemContainer(int resolution, byte[] fontData)
                {
                    _spriteFonts = new Dictionary<int, DynamicSpriteFont>();
                    _fontSystem = new FontSystem(new FontSystemSettings()
                    {
                        FontResolutionFactor = resolution,
                        KernelWidth = 1,
                        KernelHeight = 1,
                    });
                    _fontSystem.AddFont(fontData);
                }

                internal DynamicSpriteFont GetSpriteFont(int size)
                {
                    if (!_spriteFonts.TryGetValue(size, out var font))
                    {
                        font = _spriteFonts[size] = _fontSystem.GetFont(size);
                    }

                    return font;
                }
            }
            private readonly Dictionary<int, FontSystemContainer> _fontSystems;

            private readonly byte[] _fontData;

            internal FontContent(byte[] data)
            {
                _fontData = data;
                _fontSystems = new Dictionary<int, FontSystemContainer>();
            }

            internal DynamicSpriteFont GetSpriteFont(int resolution, int fontSize)
            {
                if (!_fontSystems.TryGetValue(resolution, out var fontSystemContainer))
                {
                    fontSystemContainer = new FontSystemContainer(resolution, _fontData);
                    _fontSystems.Add(resolution, fontSystemContainer);
                }

                return fontSystemContainer.GetSpriteFont(fontSize);
            }
        }

        private FontManager()
        {
            _textureManager = new FontTextureManager();
            _fontFamilies = new();
        }
        internal FontContent GetFont(FontAsset font)
        {
            if (!_fontFamilies.TryGetValue(font.Path, out var fontContent))
            {
                fontContent = new FontContent(font.Data);

                _fontFamilies[font.Path] = fontContent;
            }

            return fontContent;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TextVertex : IVertex2D<TextVertex>
    {
        public vec2 Position;
        public vec2 UV;
        public uint Color;
        private int _textureIndex;
        public int VertexIndex;

        public int TextureIndex { get => _textureIndex; set => _textureIndex = value; }

        private static unsafe VertexAtrib[] _attribs =
        [
            new VertexAtrib() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(TextVertex), Offset = 0 }, // Position
            new VertexAtrib() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(TextVertex), Offset = sizeof(float) * 2 }, // UV
            new VertexAtrib() { Count = 1, Normalized = false, Type = GfxValueType.Uint, Stride = sizeof(TextVertex), Offset = sizeof(uint) * 4 }, // Color
            new VertexAtrib() { Count = 1, Normalized = false, Type = GfxValueType.Int, Stride = sizeof(TextVertex), Offset = sizeof(int) * 5 }, // texIndex
            new VertexAtrib() { Count = 1, Normalized = false, Type = GfxValueType.Int, Stride = sizeof(TextVertex), Offset = sizeof(int) * 6 }, // charIndex
        ];

       
        static VertexAtrib[] IVertex<TextVertex>.GetVertexAttributes()
        {
            return _attribs;
        }
    }
}
