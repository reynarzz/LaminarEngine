using Engine;
using Microsoft.Build.Framework;
using SharedTypes;
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

        public EditorProjectBuildStage() : base(new BuildLogger()
        {
             DebugStatus = false
        })
        { }

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

        public override bool ShouldExecute()
        {
            var currentGameDllFolder = EditorPaths.HookFolderAbsolutePath;

            if (!Directory.Exists(currentGameDllFolder))
            {
                Directory.CreateDirectory(currentGameDllFolder);
                return true;
            }

            if (!File.Exists(EditorPaths.GameHookDLLAbsolutePath))
            {
                return true;
            }

            // Check all the files to verify of any changed.
            var outputTime = File.GetLastWriteTimeUtc(EditorPaths.GameHookDLLAbsolutePath);

            if (!File.Exists(EditorPaths.CompiledGameDllAbsolutePath))
            {
                Debug.Log($"Original '{EditorPaths.GAME_PROJECT_NAME}.dll' is non existent.");
                return true;
            }

            var files = Directory.EnumerateFiles(Paths.GetAssetsFolderPath(), "*.cs", SearchOption.AllDirectories);
            return files.Any(f => File.GetLastWriteTimeUtc(f) > outputTime);
        }
    }
}
