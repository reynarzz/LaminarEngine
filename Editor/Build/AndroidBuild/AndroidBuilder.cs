using Editor.Data;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class AndroidBuilder : PlatformBuilder
    {
        private readonly AndroidInstallBuildStage _installBuildStage = new();
        public AndroidBuilder() : base([new AndroidAssetsBuildStage(),
                                        new AndroidShipBuildStage()])
        {
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

            var androidSettings = EditorProjectDataManager.BuildSettings.GetBuildSettings(PlatformBuild.Android) as AndroidBuildSettings;

            if (androidSettings.RunAfterBuild)
            {
                AddStage(_installBuildStage);
            }

            PerformOp(() => Directory.Delete(EditorPaths.AndroidShipFolderRoot, true));
            PerformOp(() => Directory.Delete(EditorPaths.AndroidPublishFolderRoot, true));
        }

        protected override void OnAfterBuild(BuildResult result)
        {
            RemoveStage(_installBuildStage);

            if (result.IsSuccess)
            {
                EditorFileDialog.DisplayFolder(EditorPaths.AndroidShipFolderRoot);

                // TODO: Check if the user requested to install the app automatically, if so install it.
            }
        }
    }
}