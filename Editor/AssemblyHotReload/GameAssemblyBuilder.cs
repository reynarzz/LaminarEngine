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
        public event Action<bool, bool> OnBuildCompleted;
        
        private ProjectCollection _pc;
        private ProjectInstance _instance;
        private BuildParameters _parameters;
     

        private readonly Dictionary<string, string> _globalProps = new()
        {
            ["MSBuildProjectExtensionsPath"] = @"Library/Build/obj/",
            ["BaseOutputPath"] = @"Library/Build/bin/",
            ["AppendRuntimeIdentifierToOutputPath"] = "false",
            ["AppendTargetFrameworkToOutputPath"] = "false",
            ["Configuration"] = EditorPaths.GAME_BUILD_TYPE
        };

        internal GameAssemblyBuilder()
        {
            MSBuildLocator.RegisterDefaults();
        }

        internal void Build()
        {
            if (!IsBuildNeeded())
            {
                OnBuildCompleted?.Invoke(true, false);
                return;
            }

            _pc = new ProjectCollection(_globalProps);
            var project = _pc.LoadProject(Path.Combine(EditorPaths.GameRoot, EditorPaths.GAME_PROJECT_FULL_NAME));

            // Logger
            _parameters = new BuildParameters(_pc)
            {
                Loggers = [new BuildLogger()]
            };

            _instance = project.CreateProjectInstance();

            var assetsPath = Path.Combine(EditorPaths.GameRoot, "Library/Build/obj/project.assets.json");

            if (!File.Exists(assetsPath))
            {
                // Restore packages, such as nugget.
                BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, ["Restore"]));
            }

            // Compile
            var result = BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, ["Build"]));

            if (result.OverallResult == BuildResultCode.Success)
            {
                Debug.Success("Build success");
                OnBuildCompleted?.Invoke(true, true);
            }
            else
            {
                OnBuildCompleted?.Invoke(false, false);
                Debug.Error("Build failed");
            }
        }

        private bool IsBuildNeeded()
        {
            var currentGameDllFolder = EditorPaths.CurrentFolderAbsolutePath;

            if (!Directory.Exists(currentGameDllFolder))
            {
                Directory.CreateDirectory(currentGameDllFolder);
                return true;
            }

            var output = EditorPaths.GameHookDLLAbsolutePath;
            if (!File.Exists(output))
            {
                return true;
            }

            // Check all the files to verify of any changed.
            var outputTime = File.GetLastWriteTimeUtc(output);
            var files = Directory.EnumerateFiles(Paths.GetAssetsFolderPath(), "*.cs", SearchOption.AllDirectories);
            return files.Any(f => File.GetLastWriteTimeUtc(f) > outputTime);
        }

    }
}
