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

            props["LAM_APPLICATION_PROJECT_PATH"] = Path.GetFullPath(EditorPaths.GameProjectAbsolutePath);
            props["LAM_ENGINE_PROJECT_PATH"] = EditorPaths.EngineCsProjFullPath;
            props["LAM_GENERATED_PROJECT"] = EditorPaths.GameGeneratedProjectCsProjFileFullPath;
            props["LAM_TRIMMER_LINK_RD_FILE"] = EditorPaths.GameGeneratedLinkerRDFileFullPath;

            return new Dictionary<string, string>(props)
            {
                ["ShipBuild"] = "true",
                [DefaultPropertyConsts.DEFINE_CONSTANTS] = constants + ";SHIP_BUILD"
            };
        }

        protected abstract void GetAllBuildProperties(out Dictionary<string, string> props, out PlatformBuildSettings settings);
    }
}
