using GlmNet;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class WindowsProjectBuildStage : ProjectBuildStage
    {
        private readonly string[] _targets = ["Publish"];

        private readonly string[] _buildFilesExt = { ".exe", ".pdb" };

        public string CurrentOutputPath { get; private set; }

        public WindowsProjectBuildStage() : base(new BuildLogger()
        {
            DebugStatus = true
        })
        { }

        protected override Dictionary<string, string> GetBuildProperties()
        {
            var settings = GetBuildSettings<WindowsBuildSettings>(PlatformBuild.Windows);
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();

            CurrentOutputPath = string.Empty;

            if (!string.IsNullOrEmpty(buildTypeSettings.OutputPath))
            {
                CurrentOutputPath = Paths.ClearPathSeparation(buildTypeSettings.OutputPath);
            }

            return new()
            {
                ["Configuration"] = settings.Type == BuildType.Release ? "Release" : "Debug",
                ["Platform"] = "AnyCPU",

                // Publish settings
                ["RuntimeIdentifier"] = "win-x64",
                ["SelfContained"] = "true",

                // Trimming
                ["PublishTrimmed"] = "true",
                ["TrimMode"] = "full", // or 'link'

                // Output
                ["PublishDir"] = EditorPaths.Win32PublishFolderRoot + "\\",

                ["PublishSingleFile"] = "true",
                ["IncludeNativeLibrariesForSelfExtract"] = "true",
                ["EnableCompressionInSingleFile"] = "true",
                // ["DefineConstants"] = "$(DefineConstants);WINDOWS;WIN32;DESKTOP"

                // Metadata
                ["Company"] = buildTypeSettings.Company,
                ["Product"] = buildTypeSettings.ApplicationName,
                ["Description"] = buildTypeSettings.Description,
                ["Authors"] = buildTypeSettings.Authors,

                ["AssemblyTitle"] = buildTypeSettings.ApplicationName,
                ["AssemblyDescription"] = buildTypeSettings.Description,

                ["AssemblyVersion"] = GetVersion( buildTypeSettings.Version),
                ["FileVersion"] = GetVersion( buildTypeSettings.Version),
                ["InformationalVersion"] = GetVersion(buildTypeSettings.Version)
            };
        }

        private string GetVersion(ivec2 version)
        {
            return $"{version.x}.{version.y}";
        }

        protected override void OnBuildSuccess()
        {
            var rootOutputFolder = EditorPaths.ShipWin32FolderRoot;
            if (!string.IsNullOrEmpty(CurrentOutputPath))
            {
                rootOutputFolder = CurrentOutputPath;
            }

            Directory.CreateDirectory(rootOutputFolder);

            var settings = GetBuildSettings<WindowsBuildSettings>(PlatformBuild.Windows);
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();

            // Rename executable
            File.Move(Path.Combine(EditorPaths.Win32PublishFolderRoot, EditorPaths.DESKTOP_PROJECT_NAME + ".exe"),
                      Path.Combine(EditorPaths.Win32PublishFolderRoot, $"{buildTypeSettings.ApplicationName}.exe"), true);

            // Copy build files
            foreach (var file in Directory.EnumerateFiles(EditorPaths.Win32PublishFolderRoot))
            {
                if (_buildFilesExt.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                {
                    File.Copy(file, Path.Combine(rootOutputFolder, Path.GetFileName(file)), overwrite: true);
                }
            }

            // Copy assemblies: TODO: copy assemblies from the plugin folder.
            var assembliesFolder = Path.Combine(rootOutputFolder, EditorPaths.WIN32_DATA_SHIP_FOLDER_NAME, "Assemblies");
            Directory.CreateDirectory(assembliesFolder);

            // Copy glfw dll
            string glfwDllName = "glfw3.dll";
            File.Copy(Path.Combine(EditorPaths.EngineWin32NativesFolderRoot, glfwDllName), Path.Combine(assembliesFolder, glfwDllName), true);

            // Copy miniaudio dll
            string miniaudioDllName = "miniaudio.dll";
            var miniaudioDllPath = Path.Combine(EditorPaths.SharedTypesRoot, "Third", "SoundFlow", "runtimes", "win-x64", "native", miniaudioDllName);
            File.Copy(miniaudioDllPath, Path.Combine(assembliesFolder, miniaudioDllName), true);
        }

        protected override string GetCSProjPath()
        {
            return Path.Combine(EditorPaths.DesktopProjectRoot, EditorPaths.DESKTOP_PROJECT_FULL_NAME);
        }

        protected override string[] GetTargetsToBuild()
        {
            return _targets;
        }
    }
}
