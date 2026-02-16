using Engine;
using StbImageSharp_Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class TextureAssetProcessor : IAssetProcessor
    {
        AssetProccesResult IAssetProcessor.Process(BinaryReader reader, AssetMeta meta, CookingPlatform platform)
        {
            StbImage.stbi_set_flip_vertically_on_load(1);

            var bytes = new byte[reader.BaseStream.Length];
            reader.BaseStream.ReadExactly(bytes, 0, bytes.Length);

            var result = ImageResult.FromMemory(bytes);

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(result.Width);
                    bw.Write(result.Height);
                    bw.Write((int)result.Comp);
                    bw.Write(result.Data);
                    bw.Flush();

                    return new AssetProccesResult()
                    {
                        IsSuccess = true,
                        Data = ms.ToArray()
                    };
                }
            }
        }
    }
}