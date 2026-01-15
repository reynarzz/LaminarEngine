using Engine.IO;
using Newtonsoft.Json;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers
{
    public class IOLayer : LayerBase
    {
        private static AssetDatabase _assetDatabase;
        internal static AssetDatabase Database => _assetDatabase; // Remove this.

        // TODO: remove all this, this is here to avoid breaking the current IO functionality. 
        public override void Initialize()
        {
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

            DiskBase disk = null;
#if DEBUG && !MOBILE
            disk = new DevModeDisk();
#else
#if !MOBILE
            disk = new ReleaseModeDisk(Paths.GetReleaseDataFolder());
#else
            disk = new ReleaseModeDisk(GFSEngine.AssetFileStream);
#endif
#endif
            disk.Initialize();

            _assetDatabase = new AssetDatabase(assetbuilder);
            _assetDatabase.Initialize(disk);
        }

        private protected void InitializeIO(DiskBase disk, Dictionary<AssetType, AssetBuilderBase> assetsBuilder)
        {
            _assetDatabase = new AssetDatabase(assetsBuilder);

            disk.Initialize();

            _assetDatabase.Initialize(disk);
        }

        internal static AssetDatabase GetDatabase() // Refactor
        {
            return _assetDatabase;
        }

        public override void Close()
        {
        }
    }
}
