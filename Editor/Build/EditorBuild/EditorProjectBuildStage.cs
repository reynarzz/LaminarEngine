using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class EditorProjectBuildStage : ProjectBuildStage
    {
        private readonly string[] _targets = ["Build"];
        protected override Dictionary<string, string> GetBuildProperties()
        {
            return new()
            {
                ["MSBuildProjectExtensionsPath"] = @"Library/Build/obj/",
                ["BaseOutputPath"] = @"Library/Build/bin/",
                ["AppendRuntimeIdentifierToOutputPath"] = "false",
                ["AppendTargetFrameworkToOutputPath"] = "false",
                ["Configuration"] = EditorPaths.GAME_BUILD_TYPE,
                ["DefineConstants"] = "DEBUG;EDITOR;DESKTOP"
            };
        }

        protected override string GetCSProjPath()
        {
            return Path.Combine(EditorPaths.GameRoot, EditorPaths.GAME_PROJECT_FULL_NAME);
        }

        protected override string[] GetTargetsToBuild()
        {
            return _targets;
        }
    }
}
