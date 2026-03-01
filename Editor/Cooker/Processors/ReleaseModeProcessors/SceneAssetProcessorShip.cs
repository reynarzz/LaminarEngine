using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{

    internal class SceneAssetProcessorShip : AssetBinaryProcessorReleaseBase<SceneIR>
    {
        protected override void WriteBinary(BinaryWriter writer, SceneIR asset)
        {
            BinaryIRSerializer.Serialize(writer, asset);
        }
    }
}
