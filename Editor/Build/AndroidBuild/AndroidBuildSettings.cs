using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    public enum AndroidIconSizes
    {
        mdpi = 48,
        hdpi = 72,
        xhdpi = 96,
        xxhdpi = 144,
        xxxhdpi = 192,
    }

    internal class AndroidIcon
    {
        [SerializedField] public Texture2D Icon { get; set; }
        [SerializedField] public Texture2D ForegroundIcon { get; set; }
        [SerializedField] public Texture2D BackgroundIcon { get; set; }
    }

    internal class AndroidBuildTypeSettings : BuildTypeSettings
    {
        [SerializedField] public AndroidIcon DefaultIcon { get; private set; } = new();
        [SerializedField] public AndroidIcon[] Icons { get; private set; } = new AndroidIcon[Enum.GetValues<AndroidIconSizes>().Length];
        [SerializedField] public string PackageName { get; set; } = AndroidConsts.DEFAULT_APP_PACKAGE_NAME;
    }

    internal class AndroidBuildSettings : PlatformBuildSettings<AndroidBuildTypeSettings>
    {
        [PropertyHeader("Signing")]
        [SerializedField] public string KeyAlias { get; set; } = "myalias";
        [SerializedField] public string KeyPass { get; set; } = "mypassword";
        [SerializedField] public string StorePass { get; set; } = "storepassword";
        [SerializedField] public bool LaunchAfterBuild { get; set; }
    }

}
