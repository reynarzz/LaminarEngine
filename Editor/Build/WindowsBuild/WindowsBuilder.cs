using Engine;
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