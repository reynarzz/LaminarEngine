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

        private ProjectInstance _instance;
        private BuildParameters _parameters;

        internal static bool IsBuilding { get; private set; } = false;
        internal static bool IsError { get; private set; } = false;

        private readonly Dictionary<string, string> _editorBuildProps = new()
        {
            ["MSBuildProjectExtensionsPath"] = @"Library/Build/obj/",
            ["BaseOutputPath"] = @"Library/Build/bin/",
            ["AppendRuntimeIdentifierToOutputPath"] = "false",
            ["AppendTargetFrameworkToOutputPath"] = "false",
            ["Configuration"] = EditorPaths.GAME_BUILD_TYPE,
            ["DefineConstants"] = "DEBUG;EDITOR;DESKTOP"
        };

        private Dictionary<string, string> _androidBuildProps = new Dictionary<string, string>
        {
            ["Configuration"] = "Release",
            ["Platform"] = "AnyCPU",
            ["AndroidSdkDirectory"] = GetAndroidSdkPath(),
            ["AndroidKeyStore"] = "false",
            ["AndroidSigningKeyAlias"] = "myalias",
            ["AndroidSigningKeyPass"] = "mypassword",
            ["AndroidSigningStorePass"] = "storepassword",
            ["OutputPath"] = EditorPaths.ShipAndroidFolderRoot
        };

        static GameAssemblyBuilder()
        {
            _mainContext = SynchronizationContext.Current;
            MSBuildLocator.RegisterDefaults();
        }

        private static string GetAndroidSdkPath()
        {
            var sdkPath = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT") ?? Environment.GetEnvironmentVariable("ANDROID_HOME");

            if (string.IsNullOrEmpty(sdkPath))
                throw new Exception("Android SDK path not found. Check Visual Studio Android settings.");

            return sdkPath;
        }

        protected virtual void OnBeforeBuild() { }
        protected virtual void OnAfterBuild() { }
        internal Task BuildAsync()
        {
            if (IsBuilding)
            {
                return Task.CompletedTask;
            }

            return Task.Run(() =>
            {
                try
                {
                    Build(PlatformBuild.Editor);
                }
                finally
                {
                    IsBuilding = false;
                }
            });
        }

        public enum PlatformBuild
        {
            Editor,
            Windows,
            MacOs,
            Android,
            IOS,
            Linux
        }

        private ProjectCollection GetProjectCollection(PlatformBuild platform)
        {
            switch (platform)
            {
                case PlatformBuild.Editor:
                    return new ProjectCollection(_editorBuildProps);
                case PlatformBuild.Windows:
                    break;
                case PlatformBuild.MacOs:
                    break;
                case PlatformBuild.Android:
                    return new ProjectCollection(_androidBuildProps);
                case PlatformBuild.IOS:
                    break;
                case PlatformBuild.Linux:
                    break;
                default:
                    return null;
            }

            return null;
        }

        private string GetProjectToBuildPath(PlatformBuild platform)
        {
            switch (platform)
            {
                case PlatformBuild.Editor:
                    return Path.Combine(EditorPaths.GameRoot, EditorPaths.GAME_PROJECT_FULL_NAME);
                case PlatformBuild.Windows:
                    break;
                case PlatformBuild.MacOs:
                    break;
                case PlatformBuild.Android:
                    return Path.Combine(EditorPaths.AndroidProjectRoot, EditorPaths.ANDROID_PROJECT_FULL_NAME);
                case PlatformBuild.IOS:
                    break;
                case PlatformBuild.Linux:
                    break;
                default:
                    break;
            }

            return string.Empty;
        }
        internal void Build(PlatformBuild platform)
        {
            if (IsBuilding)
                return;

            IsBuilding = true;

            if (!IsBuildNeeded(platform))
            {
                IsError = false;
                RaiseBuildCompleted(true, false);
                return;
            }

            var projectCollection = GetProjectCollection(platform);
            Project project = null;
            
            project = projectCollection.LoadProject(GetProjectToBuildPath(platform));

            // Logger
            _parameters = new BuildParameters(projectCollection)
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

            BuildResult result = null;

            if (platform == PlatformBuild.Android)
            {
                result = BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, ["SignAndroidPackage"]));
            }
            else if (platform == PlatformBuild.Editor)
            {
                // Compile
                result = BuildManager.DefaultBuildManager.Build(_parameters, new BuildRequestData(_instance, ["Build"]));
            }

            if (result.OverallResult == BuildResultCode.Success)
            {
                Debug.Success("Build success");
                IsError = false;
                RaiseBuildCompleted(true, true);
            }
            else
            {
                IsError = true;
                RaiseBuildCompleted(false, false);
                Debug.Error("Build failed");
            }

            IsBuilding = false;
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

        private bool IsBuildNeeded(PlatformBuild platform)
        {
            if (platform != PlatformBuild.Editor)
                return true;

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
