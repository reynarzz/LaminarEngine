using Engine;
using Engine.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class TextAssetBuilder : IAssetBuilder<TextAsset, AssetMeta>
    {
        public TextAsset BuildAsset(ref readonly AssetInfo info, AssetMeta meta, BinaryReader reader)
        {
            return new TextAsset(ReadText(reader), meta.GUID);
        }

        public void UpdateAsset(ref readonly AssetInfo info, TextAsset asset, AssetMeta meta, BinaryReader reader)
        {
            asset.UpdateResource(ReadText(reader), meta.GUID);
        }

        private string ReadText(BinaryReader reader)
        {
            var length = (int)reader.BaseStream.Length;
            var data = new byte[length];
            reader.BaseStream.ReadExactly(data, 0, length);
            return Encoding.UTF8.GetString(data, 0, length);
        }
    }
}