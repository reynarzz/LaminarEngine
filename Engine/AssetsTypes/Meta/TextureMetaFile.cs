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
    public enum TextureWrapMode
    {
        Clamp,
        Repeat
    }
    public enum TextureFilterMode
    {
        Nearest,
        Linear
    }

    public enum TextureType
    {
        Texture,
        Texture2D,
        Texture3D,
        Sprite,
        CubeMap,
    }
}


namespace Engine
{
    public class TextureConfig
    {
        public bool IsAtlas { get; set; }
        public TextureType Type { get; set; } = TextureType.Sprite;
        public TextureWrapMode Mode { get; set; }
        public TextureFilterMode Filter { get; set; }
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