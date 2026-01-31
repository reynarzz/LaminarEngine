using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCooker;
using SharedTypes;

namespace Editor.Build
{
    public enum AssetsBuildType
    {
        All,
        OnlyMatchingFiles,
    }

    internal abstract class AssetsBuildStage : BuildStage
    {
        private readonly CookOptions _cookOptions;
        private readonly AssetsCooker _assetsCooker;

        private readonly AssetsBuildType _assetsBuildType;
        public AssetsBuildStage(CookingPlatform platform, CookingType cookType, AssetsBuildType buildType)
        {
            _assetsCooker = new AssetsCooker();
            _assetsBuildType = buildType;

            var assetsFolderPath = Paths.GetAssetsFolderPath();
            _cookOptions = new CookOptions()
            {
                Type = cookType,
                Platform = platform,
                AssetsFolderPath = Paths.GetAssetsFolderPath(),
            };
        }

        public override async Task<BuildStageResult> Execute()
        {
            var cookData = OnBeforeBuild();
            CollecMatchingFiles();

            _cookOptions.ExportFolderPath = cookData.ExportFolderPath;
            _cookOptions.FileOptions = cookData.FileOptions;

            var result = await _assetsCooker.CookAllAsync(_cookOptions);

            return new BuildStageResult()
            {
                IsSuccess = result.IsSuccess,
                Data = result.DataInfo
            };
        }

        private void CollecMatchingFiles()
        {
            if (_assetsBuildType == AssetsBuildType.All)
            {
                _cookOptions.MatchingFiles = null;
            }
            else if (_assetsBuildType == AssetsBuildType.OnlyMatchingFiles)
            {
                // TODO: The editor will walk through all the scenes recursively and detect which assets are used,
                //       so no manual list  will be needed.
                // --_cookOptions.MatchingFiles = default;

                // NOTE: This is only provisional.
                var releaseAssetsList = default(string[]);
                if (File.Exists(Paths.GetShipAssetsFilePath()))
                {
                    releaseAssetsList = File.ReadAllText(Paths.GetShipAssetsFilePath())?.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                }
                _cookOptions.MatchingFiles = releaseAssetsList;
            }
        }

        protected class CookData
        {
            public string ExportFolderPath { get; set; }
            public CookFileOptions FileOptions { get; set; }
        }

        protected abstract CookData OnBeforeBuild();
    }
}
