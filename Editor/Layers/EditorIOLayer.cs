using Editor.Build;
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
        private bool _intiialized = false;
        public EditorIOLayer()
        {
            _instance = this;
        }

        public override Task InitializeAsync()
        {
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