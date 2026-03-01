using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Serialization;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class MaterialProcessorShip : AssetBinaryProcessorReleaseBase<MaterialIR>
    {
        protected override void WriteBinary(BinaryWriter writer, MaterialIR asset)
        {
            BinaryIRSerializer.Serialize(writer, asset);
        }
    }
}
