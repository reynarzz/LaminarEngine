using Editor.Data;
using GameCooker;
using SharedTypes;
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
                                                CookingType.ReleaseMode, 
                                                AssetsBuildType.OnlyMatchingFiles,
                                                string.Empty,
                                                new CookFileOptions()
                                                {
                                                    CompressAllFiles = false,
                                                    CompressionLevel = 12,
                                                    EncryptAllFiles = true
                                                })
        {
        }

        protected override void OnBeforeBuild()
        {
            OutputFolder = GetDataOutputDir();
        }

        private static string GetDataOutputDir()
        {
            var settings = EditorDataManager.BuildSettings.GetBuildSettings(PlatformBuild.Windows) as WindowsBuildSettings;

            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();

            if (!string.IsNullOrEmpty(buildTypeSettings.OutputPath))
            {
                return Path.Combine(buildTypeSettings.OutputPath, EditorPaths.WIN32_DATA_SHIP_FOLDER_NAME);
            }

            return EditorPaths.Win32ShipGameDataFolderRoot;
        }
    }
}
