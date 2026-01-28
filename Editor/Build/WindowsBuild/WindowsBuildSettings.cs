using Engine;
using GlmNet;
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
        [SerializedField] public string Description { get; set; } = "Application description";
        [SerializedField] public string Company { get; set; } = "My Company";
        [SerializedField] public string Authors { get; set; } = "Author";
        [SerializedField] public ivec2 Version { get; set; } = new ivec2(1, 0);
    }

    internal class WindowsBuildSettings : PlatformBuildSettings<WindowsBuildTypeSettings>
    {

    }
}
