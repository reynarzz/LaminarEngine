using Editor.Data;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class iOSBuilder : PlatformBuilder
    {
        private readonly iOSInstallBuildStage _installBuildStage = new();
        public iOSBuilder() : base([new iOSAssetsBuildStage(),
                                              new iOSShipBuildStage()])
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

            var settings = EditorProjectDataManager.BuildSettings.GetBuildSettings(PlatformBuild.IOS) as iOSBuildSettings;

            if (settings.RunAfterBuild)
            {
                AddStage(_installBuildStage);
            }
        }

        protected override void OnAfterBuild(BuildResult result)
        {
            RemoveStage(_installBuildStage);

            if (result.IsSuccess)
            {
                EditorFileDialog.DisplayFolder(EditorPaths.iOSShipFolderRoot);

                // TODO: Check if the user requested to install the app automatically, if so install it.
            }
        }
    }
}