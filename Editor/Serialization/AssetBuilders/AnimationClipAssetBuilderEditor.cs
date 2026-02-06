using Editor.Utils;
using Engine;
using Engine.IO;
using Engine.Serialization;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class AnimationClipAssetBuilderEditor : JsonBasedAssetBuilder<AnimationClip>
    {
        internal override void UpdateAsset(AssetResourceBase asset, AssetMetaFileBase meta, BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}