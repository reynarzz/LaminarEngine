using Editor.Utils;
using Engine;
using Engine.IO;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    internal class SceneAssetBuilderEditor : IAssetBuilder<SceneAsset, DefaultMetaFile>
    {
        public SceneAsset BuildAsset(ref readonly AssetInfo info, DefaultMetaFile meta, BinaryReader reader)
        {
            var length = reader.BaseStream.Length;
            var data = new byte[length];
            int bytesRead = reader.BaseStream.Read(data, 0, (int)length);
            string text = Encoding.UTF8.GetString(data, 0, bytesRead);

            var sceneIr = EditorJsonUtils.Deserialize<SceneIR>(text);

            return new SceneAsset(sceneIr, info.Path, meta.GUID);
        }

        public void UpdateAsset(ref readonly AssetInfo info, SceneAsset asset, DefaultMetaFile meta, BinaryReader reader)
        {
        }
    }
}
