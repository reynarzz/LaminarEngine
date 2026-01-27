using Engine;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class EditorBuilder : PlatformBuilder
    {
        internal EditorBuilder() : base([new EditorProjectBuildStage(),
                                          new EditorAssetsBuildStage()])
        {
        }
    }
}
