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

    internal class AssetsBuildStage : BuildStage
    {
        private readonly CookOptions _cookOptions;
        private readonly AssetsCooker _assetsCooker;
        private readonly CookFileOptions _fileOptions;

        protected string OutputFolder { get; set; }
        private readonly AssetsBuildType _assetsBuildType;
        public AssetsBuildStage(CookingPlatform platform, CookingType cookType, AssetsBuildType buildType, string outPath, CookFileOptions options)
        {
            _assetsCooker = new AssetsCooker();
            _fileOptions = options;
            _assetsBuildType = buildType;
            OutputFolder = outPath;

            var assetsFolderPath = Paths.GetAssetsFolderPath();
            _cookOptions = new CookOptions()
            {
                Type = cookType,
                Platform = platform,
                AssetsFolderPath = Paths.GetAssetsFolderPath(),
                ExportFolderPath = OutputFolder,
                FileOptions = options,
            };
        }

        public override async Task<BuildStageResult> Execute()
        {
            OnBeforeBuild();
            CollecMatchingFiles();

            _cookOptions.ExportFolderPath = OutputFolder;

            var assetDatabaseInfo = await _assetsCooker.CookAllAsync(_cookOptions);

            return new BuildStageResult()
            {
                IsSuccess = true,
                Data = assetDatabaseInfo
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

        protected virtual void OnBeforeBuild() { }
    }
}
