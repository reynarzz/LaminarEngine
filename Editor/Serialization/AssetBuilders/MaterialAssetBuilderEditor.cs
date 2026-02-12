using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class MaterialAssetBuilderEditor : JsonBasedAssetBuilder<Material, AssetMeta>
    {
        public override void UpdateAsset(ref readonly AssetInfo info, Material asset, AssetMeta meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
