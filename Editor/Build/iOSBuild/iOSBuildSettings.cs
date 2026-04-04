using Engine;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class iOSBuildTypeSettings : BuildTypeSettings
    {
        [SerializedField] public Texture2D Icon { get; set; }
        [SerializedField] public string Description { get; set; } = "Application description";
        [SerializedField] public string Company { get; set; } = "My Company";
        [SerializedField] public string Authors { get; set; } = "Author";
        [SerializedField] public ivec3 ShortVersion { get; set; } = new ivec3(1, 0, 0);
        [SerializedField] public uint BundleVersion { get; set; } = 1;

        [SerializedField] public string CodesignKey { get; set; } = string.Empty;
        [SerializedField] public string ProvisioningProfile { get; set; } = string.Empty;
        [SerializedField] public string PackageName { get; set; } = "com.reynarzz.gfs";
    }

    internal class iOSBuildSettings : PlatformBuildSettings<iOSBuildTypeSettings>
    {
        [SerializedField] internal DeviceOrientation _orientation = DeviceOrientation.LandscapeAny;
        
    }
}
