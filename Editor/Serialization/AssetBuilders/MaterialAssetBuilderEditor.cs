using Engine;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class MaterialAssetBuilderEditor : JsonBasedAssetBuilder<Material>
    {
        internal override void UpdateAsset(AssetResourceBase asset, AssetMetaFileBase meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
