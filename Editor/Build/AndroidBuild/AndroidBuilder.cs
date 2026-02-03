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
        public AndroidBuilder() : base([new AndroidAssetsBuildStage(),
                                        new AndroidProjectBuildStage(),
                                        new AndroidInstallBuildStage()])
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

            PerformOp(() => Directory.Delete(EditorPaths.AndroidShipFolderRoot, true));
            PerformOp(() => Directory.Delete(EditorPaths.AndroidPublishFolderRoot, true));
        }

        protected override void OnAfterBuild(BuildResult result)
        {
            if (result.IsSucess)
            {
                EditorFileDialog.DisplayFolder(EditorPaths.AndroidShipFolderRoot);

                // TODO: Check if the user requested to install the app automatically, if so install it.
            }
        }
    }
}