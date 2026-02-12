using Engine.Serialization;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class AnimationClipAssetBuilder : IAssetBuilder<AnimationClip, AssetMeta>
    {
        public AnimationClip BuildAsset(ref readonly AssetInfo info, AssetMeta meta, BinaryReader reader)
        {
            // TODO: implement for binary based derialization, the IR reconstruction. 
            var length = reader.BaseStream.Length;
            var data = new byte[length];
            int bytesRead = reader.BaseStream.Read(data, 0, (int)length);

            var anim = new AnimationClip(info.Path, meta.GUID);

            // TODO: populate anim data.

            Deserializer.Deserialize(anim, null);

            return anim;
        }

        public void UpdateAsset(ref readonly AssetInfo info, AnimationClip asset, AssetMeta meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
