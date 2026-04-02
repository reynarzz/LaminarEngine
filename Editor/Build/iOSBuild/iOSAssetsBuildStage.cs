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
    internal class iOSAssetsBuildStage : AssetsBuildStage
    {
        public iOSAssetsBuildStage() : base(CookingPlatform.IOS, 
                                            CookingType.ShipMode,
                                            AssetsBuildType.OnlyMatchingFiles)
        {
        }

        protected override CookData OnBeforeBuild()
        {
            var settings = EditorProjectDataManager.BuildSettings.GetBuildSettings(PlatformBuild.IOS) as iOSBuildSettings;
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();

            return new()
            {
                ExportFolderPath = EditorPaths.iOSProjectAssetsFolderRoot,
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
