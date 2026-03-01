using Engine;
using Engine.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    internal class SceneAssetBuilderEditor : IAssetBuilder<SceneAsset, DefaultMetaFile>
    {
        public SceneAsset BuildAsset(ref readonly AssetInfo info, DefaultMetaFile meta, BinaryReader reader)
        {
            return new SceneAsset(info.Path, meta.GUID);
        }

        public void UpdateAsset(ref readonly AssetInfo info, SceneAsset asset, DefaultMetaFile meta, BinaryReader reader)
        {
        }
    }
}
