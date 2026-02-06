using Engine.Serialization;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class AnimationClipAssetBuilder : AssetBuilderBase
    {
        internal override AssetResourceBase BuildAsset(AssetInfo info, AssetMetaFileBase meta, Guid guid, BinaryReader reader)
        {
            // TODO: implement for binary based derialization, the IR reconstruction. 
            var length = reader.BaseStream.Length;
            var data = new byte[length];
            int bytesRead = reader.BaseStream.Read(data, 0, (int)length);

            var anim = new AnimationClip(info.Path, guid);

            // TODO: populate anim data.

            Deserializer.Deserialize(anim, null);

            return anim;
        }

        internal override void UpdateAsset(AssetResourceBase asset, AssetMetaFileBase meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
