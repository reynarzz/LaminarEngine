using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class SceneAssetBuilder : IAssetBuilder<SceneAsset, DefaultMetaFile>
    {
        public SceneAsset BuildAsset(ref readonly AssetInfo info, DefaultMetaFile meta, BinaryReader reader)
        {
            var sceneIr = BinaryIRDeserializer.DeserializeScene(reader);
            return new SceneAsset(sceneIr, info.Path, meta.GUID);
        }

        public void UpdateAsset(ref readonly AssetInfo info, SceneAsset asset, DefaultMetaFile meta, BinaryReader reader)
        {
        }
    }
}
