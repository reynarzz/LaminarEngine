using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class WindowsBuildTypeSettings : BuildTypeSettings
    {
        [SerializedField] public Texture2D Icon { get; set; }
        [SerializedField] public string Description { get; set; }
        [SerializedField] public string Company { get; set; }
        [SerializedField] public string Authors { get; set; }
    }

    internal class WindowsBuildSettings : PlatformBuildSettings<WindowsBuildTypeSettings>
    {

    }
}
