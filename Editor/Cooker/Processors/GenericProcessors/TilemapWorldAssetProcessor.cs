using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class TilemapWorldAssetProcessor : IAssetProcessor
    {
        AssetProccesResult IAssetProcessor.Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            
            return new AssetProccesResult()
            {
                IsSuccess = true,
                //Data = Encoding.UTF8.GetBytes(reader.ReadToEnd())
            };
        }
    }
}