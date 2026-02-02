using Engine;
using GlmNet;
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

    public enum AndroidApiLevel
    {
        Android_6_Marshmallow_API_23 = 23,
        Android_7_Nougat_API_24 = 24,
        Android_7_1_NougatMr1_API_25 = 25,
        Android_8_Oreo_API_26 = 26,
        Android_8_1_OreoMr1_API_27 = 27,
        Android_9_Pie_API_28 = 28,
        Android_10_API_29 = 29,
        Android_11_API_30 = 30,
        Android_12_API_31 = 31,
        Android_12L_API_32 = 32,
        Android_13_API_33 = 33,
        Android_14_API_34 = 34,
        Android_15_API_35 = 35
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
        [SerializedField] public AndroidApiLevel MinimumApiLevel { get; set; }
        [SerializedField] public AndroidApiLevel TargetApiLevel { get; set; } = Enum.GetValues<AndroidApiLevel>()[^1];
        [SerializedField] public ivec3 Version { get; set; } = new ivec3(1, 0, 0);
    }
}
