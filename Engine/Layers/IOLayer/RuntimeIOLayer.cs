using Engine.IO;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    internal class RuntimeIOLayer : IOLayer
    {
        public override void Initialize()
        {
            DiskBase disk = null;
#if !MOBILE
            disk = new ReleaseModeDisk(Paths.GetReleaseDataFolder());
#else
            disk = new ReleaseModeDisk(GFSEngine.AssetFileStream);
#endif
            var assetbuilder = new Dictionary<AssetType, AssetBuilderBase>()
            {
                { AssetType.Texture, new TextureAssetBuilder() },
                { AssetType.Text, new TextAssetBuilder() },
                { AssetType.Shader, new TextAssetBuilder() },
                { AssetType.Audio, new AudioClipAssetBuilder() },
                { AssetType.Font, new FontAssetBuilder() },
                { AssetType.AnimationClip, new AnimationClipAssetBuilder() },
                //{ AssetType.AnimationController, new FontAssetBuilder() },
            };

            InitializeIO(disk, assetbuilder);
        }
    }
}
