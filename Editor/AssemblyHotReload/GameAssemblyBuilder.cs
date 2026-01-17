using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Engine;
using SharedTypes;

namespace Editor.AssemblyHotReload
{
    internal class GameAssemblyBuilder
    {
        public event Action<bool> BuildCompleted;
        private const string BUILD_TYPE = "Debug";
        private ProjectCollection _pc;
        private ProjectInstance _instance;
        private BuildParameters _parameters;
        private readonly Dictionary<string, string> _globalProps = new ()
        {
            ["MSBuildProjectExtensionsPath"] = @"Library/Build/obj/",
            ["BaseOutputPath"] = @"Library/Build/bin/",
            ["AppendRuntimeIdentifierToOutputPath"] = "false",
            ["AppendTargetFrameworkToOutputPath"] = "false",
            ["Configuration"] = BUILD_TYPE
        };

        internal GameAssemblyBuilder()
        {
            MSBuildLocator.RegisterDefaults();
        }

        internal void Build()
        {
            if (!NeedsBuild())
            {
                BuildCompleted?.Invoke(true);
                return;
            }

            _pc = new ProjectCollection(_globalProps);
            var project = _pc.LoadProject(Path.Combine(EditorPaths.GameRoot, EditorPaths.GAME_PROJECT_FULL_NAME));

            _parameters = new BuildParameters(_pc)
            {
                Loggers = [new BuildLogger()]
            };

            _instance = project.CreateProjectInstance();

            var assetsPath = Path.Combine(EditorPaths.GameRoot, "Library/Build/obj/project.assets.json");

            if (!File.Exists(assetsPath))
            {
                BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, ["Restore"]));
            }

            var result = BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, ["Build"]));

            if (result.OverallResult == BuildResultCode.Success)
            {
                Debug.Success("Build success");
                BuildCompleted?.Invoke(true);
            }
            else
            {
                BuildCompleted?.Invoke(false);
                Debug.Error("Build failed");
            }
        }

        private bool NeedsBuild()
        {
            var gameDll = $@"Library/Build/bin/{BUILD_TYPE}/{EditorPaths.GAME_PROJECT_NAME}.dll";
            var output = Path.Combine(EditorPaths.GameRoot, gameDll);
            if (!File.Exists(output))
                return true;

            var outputTime = File.GetLastWriteTimeUtc(output);
            var files = Directory.EnumerateFiles(Paths.GetAssetsFolderPath(), "*.cs", SearchOption.AllDirectories);
            return files.Any(f => File.GetLastWriteTimeUtc(f) > outputTime);
        }

    }
}
