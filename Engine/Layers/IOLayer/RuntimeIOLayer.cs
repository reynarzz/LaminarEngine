using Engine.IO;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    internal class RuntimeIOLayer : IOLayer
    {
        public override Task InitializeAsync()
        {
            DiskBase disk = null;
#if DESKTOP
            disk = new ReleaseModeDisk(Paths.GetReleaseDataFolder());
#elif MOBILE
            disk = new ReleaseModeDisk(GFSEngine.AssetFileStream);
#endif
            var assetbuilder = new Dictionary<AssetType, IAssetBuilder>()
            {
                { AssetType.Texture, new TextureAssetBuilder() },
                { AssetType.Text, new TextAssetBuilder() },
                { AssetType.Shader, new TextAssetBuilder() },
                { AssetType.ShaderV2, new ShaderAssetBuilder() },
                { AssetType.Audio, new AudioClipAssetBuilder() },
                { AssetType.Font, new FontAssetBuilder() },
                { AssetType.AnimationClip, new AnimationClipAssetBuilder() },
                { AssetType.Material, new MaterialAssetBuilder() },
                //{ AssetType.AnimationController, new FontAssetBuilder() },
                { AssetType.Tilemap, new TilemapAssetBuilder() },
                { AssetType.Scene, new SceneAssetBuilder() },
            };

            try
            {
                InitializeIO(disk, assetbuilder);
            }
            catch (Exception e)
            {
                Debug.Error(e);   
            }

            return Task.CompletedTask;
        }
    }
}
