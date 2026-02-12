using Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Fix namespace
namespace Engine
{
    public enum TextureMode
    {
        Clamp,
        Repeat
    }
    public enum TextureFilter
    {
        Nearest,
        Linear
    }
}


namespace Engine
{
    public class TextureConfig
    {
        public bool IsAtlas { get; set; }
        public TextureMode Mode { get; set; }
        public TextureFilter Filter { get; set; }
        public int PixelPerUnit { get; set; }
    }

    [Serializable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public class TextureMetaFile : AssetMeta
    {
        public TextureConfig Config { get; set; } = new TextureConfig() { PixelPerUnit = 16 };
        public TextureAtlasData AtlasData { get; set; } = new();
    }
}