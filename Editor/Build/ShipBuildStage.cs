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
            var props = GetAllBuildProperties();
            var constants = props[DefaultPropertyConsts.DEFINE_CONSTANTS];

            if (constants.EndsWith(";"))
            {
                constants = constants.TrimEnd(';');
            }

            return new Dictionary<string, string>(props)
            {
                ["ShipBuild"] = "true",
                [DefaultPropertyConsts.DEFINE_CONSTANTS] = constants + ";SHIP_BUILD"
            };
        }

        protected abstract Dictionary<string, string> GetAllBuildProperties();

    }
}
