using SharedTypes;
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
        AssetProccesResult IAssetProcessor.Process(string path, AssetMetaFileBase meta, CookingPlatform platform)
        {
            using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

            return new AssetProccesResult()
            {
                IsSuccess = true,
                Data = Encoding.UTF8.GetBytes(reader.ReadToEnd())
            };
        }
    }
}