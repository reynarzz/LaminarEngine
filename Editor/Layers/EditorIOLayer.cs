using Editor.Build;
using Editor.Data;
using Editor.Serialization;
using Engine;
using Engine.IO;
using Engine.Layers;
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
        private bool _intiialized = false;
        public EditorIOLayer()
        {
            _instance = this;
        }

        public override Task InitializeAsync()
        {
            // Editor asset builders
            InitializeIO(_devDisk, new Dictionary<AssetType, IAssetBuilder>()
            {
                { AssetType.Texture, new TextureAssetBuilder() },
                { AssetType.Text, new TextAssetBuilder() },
                { AssetType.Shader, new TextAssetBuilder() },
                { AssetType.ShaderV2, new ShaderAssetBuilderEditor() },
                { AssetType.Audio, new AudioClipAssetBuilder() },
                { AssetType.Font, new FontAssetBuilder() },
                { AssetType.AnimationClip, new AnimationClipAssetBuilderEditor() },
                { AssetType.AnimatorController, new AnimationControllerAssetBuilderEditor() },
                { AssetType.Material, new MaterialAssetBuilderEditor() },
                { AssetType.Tilemap, new TilemapAssetBuilder() },
                { AssetType.Scene, new SceneAssetBuilderEditor() },
            });

            EditorDataManager.Init();

            _intiialized = true;
            return Task.CompletedTask;
        }

        internal void ReloadDisk(AssetsDatabaseInfo assetDatabase)
        {
            if (!_intiialized)
                return;

            _devDisk.Initialize();
            Reload(_devDisk);

            // Update database.
            foreach (var guid in assetDatabase.UpdatedAssets)
            {
                Database?.UpdateReloadAsset(guid);
            }
        }

        internal void Refresh()
        {
            BuildSystem.BuildAsync(PlatformBuild.GameAppDomain);
        }
    }
}