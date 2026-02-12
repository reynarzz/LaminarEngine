using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class TextAssetProcessor : IAssetProcessor
    {
        public virtual AssetProccesResult Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            var bytes = new byte[(int)reader.BaseStream.Length];
            reader.BaseStream.ReadExactly(bytes, 0, bytes.Length);

            return new AssetProccesResult()
            {
                IsSuccess = true,
                Data = bytes
            };
        }
    }
}