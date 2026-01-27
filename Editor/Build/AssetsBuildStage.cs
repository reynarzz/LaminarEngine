using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCooker;
using SharedTypes;

namespace Editor.Build
{
    public enum AssetBuildType
    {
        All,
        OnlyMatchingFiles,
    }

    internal class AssetsBuildStage : BuildStage
    {
        private readonly CookOptions _cookOptions;
        private readonly AssetsCooker _assetsCooker;
        private readonly CookFileOptions _fileOptions;

        public AssetsBuildStage(CookingPlatform platform, CookingType cookType, AssetBuildType buildType, string outPath, CookFileOptions options)
        {
            _assetsCooker = new AssetsCooker();
            _fileOptions = options;
            var assetsFolderPath = Paths.GetAssetsFolderPath();
            _cookOptions = new CookOptions()
            {
                Type = cookType,
                Platform = platform,
                AssetsFolderPath = Paths.GetAssetsFolderPath(),
                ExportFolderPath = outPath,
                FileOptions = options,
            };

            if (buildType == AssetBuildType.All)
            {
                _cookOptions.MatchingFiles = null;
            }
            else if (buildType == AssetBuildType.OnlyMatchingFiles)
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

        public override async Task<BuildStageResult> Execute()
        {
            var assetDatabaseInfo = await _assetsCooker.CookAllAsync(_cookOptions);

            return new BuildStageResult()
            {
                IsSuccess = true,
                Data = assetDatabaseInfo
            };
        }

        public override bool ShouldBuild()
        {
            return true;
        }
    }
}
