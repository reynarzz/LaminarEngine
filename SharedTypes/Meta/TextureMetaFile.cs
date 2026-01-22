using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    [Serializable]
    public struct TextureConfig
    {
        public bool IsNearest { get; set; }
        public bool IsAtlas { get; set; }
        public int Mode { get; set; }
        public int Filter { get; set; }
        public int PixelPerUnit { get; set; }
    }

    [Serializable]
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    public class TextureMetaFile : AssetMetaFileBase
    {
        public TextureConfig Config { get; set; } = new TextureConfig() { PixelPerUnit = 16 };
        public TextureAtlasData AtlasData { get; set; } = new();
    }
}