using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class FontAssetBuilder : IAssetBuilder<FontAsset, AssetMeta>
    {
        public FontAsset BuildAsset(ref readonly AssetInfo info, AssetMeta meta, BinaryReader reader)
        {
            return new FontAsset(meta.GUID, reader.ReadBytes((int)reader.BaseStream.Length));
        }

        public void UpdateAsset(ref readonly AssetInfo info, FontAsset asset, AssetMeta meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
