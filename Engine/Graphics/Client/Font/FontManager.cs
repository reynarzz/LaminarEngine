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

        public mat4 TestUIProjection; // Remove, this should come from the UICanvas

        private readonly Dictionary<string, FontContent> _fontFamilies;
        internal class FontContent
        {
            private readonly FontSystem _fontSystem;
            private readonly Dictionary<int, DynamicSpriteFont> _spriteFonts;
            internal FontContent(FontSystem fontSystem)
            {
                _fontSystem = fontSystem;
                _spriteFonts = new();
            }

            internal DynamicSpriteFont GetSpriteFont(int size)
            {
                if(!_spriteFonts.TryGetValue(size, out var font))
                {
                    font = _spriteFonts[size] = _fontSystem.GetFont(size);
                }

                return font;
            }
        }

        private FontManager()
        {
            _textureManager = new FontTextureManager();
            _fontFamilies = new();

            var targetScreenRes = new vec2(512 * 2, 288 * 2);
            var viewMatrix = MathUtils.Ortho(0, targetScreenRes.x, 0, targetScreenRes.y, 0, -1);
            //var viewMatrix = MathUtils.Ortho(-_targetScreenRes.x / 2, _targetScreenRes.x / 2, _targetScreenRes.y / 2, -_targetScreenRes.y / 2, 0, -1);

            TestUIProjection = viewMatrix;
        }

        internal FontContent GetFont(FontAsset font)
        {
            if (!_fontFamilies.TryGetValue(font.Path, out var fontContent))
            {
                var settings = new FontSystemSettings
                {
                    FontResolutionFactor = 3,
                    KernelWidth = 2,
                    KernelHeight = 2,
                };
                var fontSystem = new FontSystem(settings);
                fontSystem.AddFont(font.Data);

                fontContent = new FontContent(fontSystem);

                _fontFamilies[font.Path] = fontContent;
            }

            return fontContent;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct FontVertex : IVertex<FontVertex>
    {
        public vec2 Position;
        public vec2 UV;
        public uint Color;
        public int VertexIndex;

        private static unsafe VertexAtrib[] _attribs =
        [
            new VertexAtrib() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(FontVertex), Offset = 0 }, // Position
            new VertexAtrib() { Count = 2, Normalized = false, Type = GfxValueType.Float, Stride = sizeof(FontVertex), Offset = sizeof(float) * 2 }, // UV
            new VertexAtrib() { Count = 1, Normalized = false, Type = GfxValueType.Uint, Stride = sizeof(FontVertex), Offset = sizeof(float) * 4 }, // Color
            new VertexAtrib() { Count = 1, Normalized = false, Type = GfxValueType.Int, Stride = sizeof(FontVertex), Offset = sizeof(float) * 5 }, // charIndex
        ];

        static VertexAtrib[] IVertex<FontVertex>.GetVertexAttributes()
        {
            return _attribs;
        }
    }
}
