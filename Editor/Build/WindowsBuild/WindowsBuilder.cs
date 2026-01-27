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
    }
}