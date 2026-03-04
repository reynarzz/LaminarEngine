using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class BuildSettings
    {
        [SerializedField] private Dictionary<PlatformBuild, PlatformBuildSettings> _platformsBuildSettings; 
        
        public BuildSettings()
        {
            _platformsBuildSettings = new()
            {
                { PlatformBuild.Windows, new WindowsBuildSettings() },
                { PlatformBuild.Android, new AndroidBuildSettings() },
            };
        }

        public PlatformBuildSettings GetBuildSettings(PlatformBuild platform)
        {
            if(_platformsBuildSettings.TryGetValue(platform, out var settings))
            {
                return settings;
            }

            Debug.Error($"Settings: '{platform}' is not implemented.");
            return null;
        }
    }
}
