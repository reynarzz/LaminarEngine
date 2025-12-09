using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    internal class RawBytesAssetProcessor : IAssetProcessor
    {
        byte[] IAssetProcessor.Process(string path, AssetMetaFileBase meta, CookingPlatform platform)
        {
            return File.ReadAllBytes(path);
        }
    }
}
