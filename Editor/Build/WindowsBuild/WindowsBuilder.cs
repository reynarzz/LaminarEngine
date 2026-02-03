using Editor.Data;
using Engine;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class WindowsBuilder : PlatformBuilder
    {
        public WindowsBuilder() : base([new WindowsProjectBuildStage(),
                                        new WindowsAssetsBuildStage()])
        {
        }

        protected override void OnAfterBuild(BuildResult result)
        {
            if (result.IsSucess)
            {
                var rootOutputFolder = EditorPaths.ShipWin32FolderRoot;

                var settings = EditorDataManager.BuildSettings.GetBuildSettings(PlatformBuild.Windows) as WindowsBuildSettings;
                var buildTypeSettings = settings.GetCurrentBuildTypeSettings();
                if (!string.IsNullOrEmpty(buildTypeSettings.OutputPath))
                {
                    rootOutputFolder = Paths.ClearPathSeparation(buildTypeSettings.OutputPath);
                }

                EditorFileDialog.DisplayFolder(rootOutputFolder);
            }
        }

        protected override void OnBeforeBuild()
        {
            void PerformOp(Action op)
            {
                try
                {
                    op();
                }
                catch (Exception e)
                {
                    Debug.Warn(e.ToString());
                }
            }

            PerformOp(() => Directory.Delete(EditorPaths.ShipWin32FolderRoot, true));
            PerformOp(() => Directory.Delete(EditorPaths.Win32PublishFolderRoot, true));
        }
    }
}