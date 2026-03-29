using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal abstract class ShipBuildStage : ProjectBuildStage
    {
        protected ShipBuildStage(ILogger logger) : base(logger) { }
        protected sealed override Dictionary<string, string> GetBuildProperties()
        {
            GetAllBuildProperties(out var props, out var settings);
            
            var constants = props[DefaultPropertyConsts.DEFINE_CONSTANTS];

            if (constants.EndsWith(";"))
            {
                constants = constants.TrimEnd(';');
            }

            if (settings.NativeAOT)
            {
                props["PublishAot"] = "true";
            }

            props["ApplicationProjectPath"] = EditorPaths.GameProjectAbsolutePath;
            props["EngineProjectPath"] = EditorPaths.EngineCsProjFullPath;

            return new Dictionary<string, string>(props)
            {
                ["ShipBuild"] = "true",
                [DefaultPropertyConsts.DEFINE_CONSTANTS] = constants + ";SHIP_BUILD"
            };
        }

        protected abstract void GetAllBuildProperties(out Dictionary<string, string> props, out PlatformBuildSettings settings);
    }
}
