using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal struct AssetProccesResult
    {
        public bool IsSuccess { get; set; }
        public byte[] Data { get; set; }
    }

    internal abstract class AssetsCookerBase
    {
        private readonly Dictionary<AssetType, IAssetProcessor> _assetProcessor;
        protected AssetsCookerBase(Dictionary<AssetType, IAssetProcessor> processor)
        {
            _assetProcessor = processor;
        }

        internal abstract Task<bool> CookAssetsAsync(CookFileOptions fileOptions, CookingPlatform platform, 
                                                     (string filePath, AssetType assetType)[] files, string outFolder);

        protected AssetProccesResult ProcessAsset(CookingPlatform platform, AssetType type, AssetMeta meta, BinaryReader reader)
        {
            if (_assetProcessor.TryGetValue(type, out var processor))
            {
                return processor.Process(reader, meta, platform);
            }

            return default;
        }
    }
}