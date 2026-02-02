using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
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

        internal abstract Task<bool> CookAssetsAsync(CookFileOptions fileOptions, CookingPlatform platform, (string, AssetType)[] files, string outFolder);

        protected AssetProccesResult ProcessAsset(CookingPlatform platform, AssetType type, AssetMetaFileBase meta, string path)
        {
            if (_assetProcessor.TryGetValue(type, out var processor))
            {
                return processor.Process(path, meta, platform);
            }

            return default;
        }
    }
}