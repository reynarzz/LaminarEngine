using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class RawBytesAssetProcessor : IAssetProcessor
    {
        AssetProccesResult IAssetProcessor.Process(string path, AssetMetaFileBase meta, CookingPlatform platform)
        {
            return new AssetProccesResult()
            {
                IsSuccess = true,
                Data = File.ReadAllBytes(path)
            };
        }
    }
}
