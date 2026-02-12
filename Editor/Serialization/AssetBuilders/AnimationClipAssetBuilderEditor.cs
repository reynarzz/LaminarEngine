using Editor.Utils;
using Engine;
using Engine.IO;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class AnimationClipAssetBuilderEditor : JsonBasedAssetBuilder<AnimationClip, AssetMeta, AnimClipIR>
    {
        public override void UpdateAsset(ref readonly AssetInfo info, AnimationClip asset, AssetMeta meta, BinaryReader reader)
        {
            
        }
    }
}