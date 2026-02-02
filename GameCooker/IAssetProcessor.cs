using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    internal interface IAssetProcessor
    {
        AssetProccesResult Process(string path, AssetMetaFileBase meta, CookingPlatform platform);
    }
}
