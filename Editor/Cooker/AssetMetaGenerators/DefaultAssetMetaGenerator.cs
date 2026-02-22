using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal interface IAssetMetaGenerator
    {
        public AssetMeta GetDefaultMeta(BinaryReader reader);
    }
    internal class DefaultAssetMetaGenerator
    {
        private readonly Dictionary<AssetType, IAssetMetaGenerator> _metasGenerators;

        public DefaultAssetMetaGenerator()
        {
            _metasGenerators = new Dictionary<AssetType, IAssetMetaGenerator>()
            {
                { AssetType.Tilemap, new TilemapMetaGenerator() },
                { AssetType.Audio, new DefaultMetaGenerator<AudioMetaFile>() },
                { AssetType.Texture, new DefaultMetaGenerator<TextureMetaFile>() },
                
            };
        }

        public AssetMeta GetDefaultAssetMeta(string assetAbsolutePath, AssetType type)
        {
            if (_metasGenerators.TryGetValue(type, out var generator))
            {
                using var fs = File.OpenRead(assetAbsolutePath);
                using var br = new BinaryReader(fs);

                var mt = generator.GetDefaultMeta(br);
                mt.GUID = Guid.NewGuid();

                return mt;
            }

            return null;
        }
    }
}
