using Editor.Data;
using Editor.Cooker;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class AndroidAssetsBuildStage : AssetsBuildStage
    {
        public AndroidAssetsBuildStage() : base(CookingPlatform.Android,
                                                CookingType.ShipMode,
                                                AssetsBuildType.OnlyMatchingFiles)
        {
        }

        protected override CookData OnBeforeBuild()
        {
            var settings = EditorDataManager.BuildSettings.GetBuildSettings(PlatformBuild.Android) as AndroidBuildSettings;
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();

            return new()
            {
                ExportFolderPath = EditorPaths.AndroidProjectAssetsFolderRoot,
                FileOptions = new CookFileOptions()
                {
                    EncryptAllFiles = buildTypeSettings.EncryptAssets,
                    CompressAllFiles = buildTypeSettings.CompressAssets,
                    CompressionLevel = buildTypeSettings.CompressionLevel,
                }
            };
        }
    }
}
