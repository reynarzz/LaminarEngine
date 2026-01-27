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
        private DevModeDisk _devDisk = new();
        private static EditorIOLayer _instance;
        public static EditorIOLayer Instance => _instance; // Dirty, please remove.
        public EditorIOLayer()
        {
            _instance = this;
        }

        public override void Initialize()
        {
            ImportAssets();

            InitializeIO(_devDisk, new Dictionary<AssetType, AssetBuilderBase>()
            {
                { AssetType.Texture, new TextureAssetBuilder() },
                { AssetType.Text, new TextAssetBuilder() },
                { AssetType.Shader, new TextAssetBuilder() },
                { AssetType.ShaderV2, new ShaderAssetBuilder() },
                { AssetType.Audio, new AudioClipAssetBuilder() },
                { AssetType.Font, new FontAssetBuilder() },
                { AssetType.AnimationClip, new AnimationClipAssetBuilderEditor() },
                { AssetType.AnimationController, new AnimationControllerAssetBuilderEditor() },
                { AssetType.Material, new MaterialAssetBuilderEditor() },
            });
        }

        private void ImportAssets()
        {
            var releaseAssetsList = default(string[]);
            if (File.Exists(Paths.GetShipAssetsFilePath()))
            {
                releaseAssetsList = File.ReadAllText(Paths.GetShipAssetsFilePath())?.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            }
            var assetDatabase = new GameCooker.AssetsCooker().CookAll(new GameCooker.CookOptions()
            {
                Type = GameCooker.CookingType.DevMode,
                Platform = GameCooker.CookingPlatform.Windows,
                AssetsFolderPath = Paths.GetAssetsFolderPath(),
                ExportFolderPath = Paths.GetAssetDatabaseFolder(),
                FileOptions = new GameCooker.CookFileOptions()
                {
                    CompressAllFiles = false,
                    CompressionLevel = 12,
                    EncryptAllFiles = false,
                },
                // TODO: The editor will walk through all the scenes recursively and detect which assets are used,
                //       so no manual list  will be needed.
                MatchingFiles = releaseAssetsList
            });

            foreach (var guid in assetDatabase.UpdatedAssets)
            {
                Database?.UpdateReloadAsset(guid);
            }
        }

        public override void OnEvent(EventType type, object value)
        {
            if(type == EventType.WindowFocusEnter)
            {
                Refresh();
            }
        }

        internal void Refresh()
        {
            ImportAssets();
            _devDisk.Initialize();
            Reload(_devDisk);
        }
    }
}