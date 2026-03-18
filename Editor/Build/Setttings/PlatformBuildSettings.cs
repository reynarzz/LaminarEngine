using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    public enum BuildType
    {
        Release,
        Debug
    }

    internal abstract class BuildTypeSettings
    {
        [SerializedField] public string ApplicationName = "AppName";
        [SerializedField] public string OutputPath;

        [PropertyHeader("Assets Config")]
        [SerializedField] public bool EncryptAssets;
        [SerializedField] public bool CompressAssets;
        [SerializedField] public int CompressionLevel;
    }

    internal abstract class PlatformBuildSettings
    {
        [SerializedField] public BuildType Type { get; set; }
        [SerializedField] public bool ShareDefaultSettings { get; set; } = true;
        public bool RunAfterBuild { get; set; }
    }

    internal abstract class PlatformBuildSettings<T> : PlatformBuildSettings where T: BuildTypeSettings, new()
    {
        [SerializedField] public T Default { get; private set; } = new T();
        [SerializedField] public T Release { get; private set; } = new T();
        [SerializedField] public T Debug { get; private set; } = new T();

        public T GetCurrentBuildTypeSettings()
        {
            if (ShareDefaultSettings)
            {
                return Default;
            }

            if(Type == BuildType.Release)
            {
                return Release;
            }

            return Debug;
        }
    }

    internal class AssetsBuildSettings
    {
        public bool EncryptAll { get; set; }
        public bool CompressAll { get; set; }
        public int CompressionLevel { get; set; }

    }
}
