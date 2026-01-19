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
        private static SynchronizationContext _mainContext;

        private ProjectCollection _pc;
        private ProjectInstance _instance;
        private BuildParameters _parameters;

        private bool _isBuilding = false;

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
            _mainContext = SynchronizationContext.Current;

            MSBuildLocator.RegisterDefaults();
        }

        internal Task BuildAsync()
        {
            if (_isBuilding)
            {
                return Task.CompletedTask;
            }

            return Task.Run(() =>
            {
                try
                {
                    Build();
                }
                finally
                {
                    _isBuilding = false;
                }
            });
        }

        internal void Build()
        {
            if (_isBuilding)
                return;

            _isBuilding = true;

            if (!IsBuildNeeded())
            {
                RaiseBuildCompleted(true, false);
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
                RaiseBuildCompleted(true, true);
            }
            else
            {
                RaiseBuildCompleted(false, false);
                Debug.Error("Build failed");
            }

            _isBuilding = false;
        }

        private void RaiseBuildCompleted(bool success, bool didBuild)
        {
            var ctx = _mainContext;

            if (ctx != null)
            {
                ctx.Post(_ => OnBuildCompleted?.Invoke(success, didBuild), null);
            }
            else
            {
                // Fallback if no context 
                OnBuildCompleted?.Invoke(success, didBuild);
            }
        }

        private bool IsBuildNeeded()
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

            if (!File.Exists(EditorPaths.CompiledGameDllAbsolutePath) ||
                File.GetLastWriteTime(EditorPaths.CompiledGameDllAbsolutePath) != outputTime)
            {
                Debug.Log($"Original '{EditorPaths.GAME_PROJECT_NAME}.dll' is non existent.");
                return true;
            }

            var files = Directory.EnumerateFiles(Paths.GetAssetsFolderPath(), "*.cs", SearchOption.AllDirectories);
            return files.Any(f => File.GetLastWriteTimeUtc(f) > outputTime);
        }

    }
}
