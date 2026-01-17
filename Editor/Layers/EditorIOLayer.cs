using Engine;
using Engine.IO;
using Engine.Layers;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class EditorIOLayer : IOLayer
    {
        public override void Initialize()
        {
            InitializeIO(new DevModeDisk(), new Dictionary<AssetType, AssetBuilderBase>()
            {
                { AssetType.Texture, new TextureAssetBuilder() },
                { AssetType.Text, new TextAssetBuilder() },
                { AssetType.Shader, new TextAssetBuilder() },
                { AssetType.ShaderV2, new ShaderAssetBuilder() },
                { AssetType.Audio, new AudioClipAssetBuilder() },
                { AssetType.Font, new FontAssetBuilder() },
                { AssetType.AnimationClip, new AnimationClipAssetBuilderEditor() },
                { AssetType.AnimationController, new AnimationControllerAssetBuilderEditor() },
            });
        }
    }
}