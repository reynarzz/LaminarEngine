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
                                        new AndroidProjectBuildStage()])
        {
        }
    }
}