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
    internal class WindowsAssetsBuildStage : AssetsBuildStage
    {
        public WindowsAssetsBuildStage() : base(CookingPlatform.Windows,
                                                CookingType.ShipMode,
                                                AssetsBuildType.OnlyMatchingFiles)
        {
        }

        protected override CookData OnBeforeBuild()
        {
            var settings = EditorDataManager.BuildSettings.GetBuildSettings(PlatformBuild.Windows) as WindowsBuildSettings;
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();

            return new CookData()
            {
                ExportFolderPath = GetDataOutputDir(buildTypeSettings),
                FileOptions = new CookFileOptions()
                {
                    EncryptAllFiles = buildTypeSettings.EncryptAssets,
                    CompressAllFiles = buildTypeSettings.CompressAssets,
                    CompressionLevel = buildTypeSettings.CompressionLevel,
                }
            };
        }

        private static string GetDataOutputDir(BuildTypeSettings buildTypeSettings)
        {
            if (!string.IsNullOrEmpty(buildTypeSettings.OutputPath))
            {
                return Path.Combine(buildTypeSettings.OutputPath, EditorPaths.WIN32_DATA_SHIP_FOLDER_NAME);
            }

            return EditorPaths.Win32ShipGameDataFolderRoot;
        }
    }
}
