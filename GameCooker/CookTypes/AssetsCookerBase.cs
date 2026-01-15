using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    internal abstract class AssetsCookerBase
    {
        private readonly Dictionary<AssetType, IAssetProcessor> _assetProcessor;
        protected AssetsCookerBase(Dictionary<AssetType, IAssetProcessor> processor)
        {
            _assetProcessor = processor;
        }

        internal abstract Task CookAssetsAsync(CookFileOptions fileOptions, CookingPlatform platform, (string, AssetType)[] files, string outFolder);

        protected byte[] ProcessAsset(CookingPlatform platform, AssetType type, AssetMetaFileBase meta, string path)
        {
            if (_assetProcessor.TryGetValue(type, out var processor))
            {
                return processor.Process(path, meta, platform);
            }

            return [];
        }
    }
}