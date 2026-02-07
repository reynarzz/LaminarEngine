using GlmNet;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Build
{
    internal class WindowsShipBuildStage : ShipBuildStage
    {
        private readonly string[] _targets = ["Rebuild", "Publish"];

        private readonly string[] _buildFilesExt = { ".exe", ".pdb" };

        public WindowsShipBuildStage() : base(new BuildLogger()
        {
            DebugStatus = true
        })
        { }

        protected override Dictionary<string, string> GetAllBuildProperties()
        {
            var settings = GetBuildSettings<WindowsBuildSettings>(PlatformBuild.Windows);
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();

            var props = new Dictionary<string, string>()
            {
                ["Configuration"] = settings.Type == BuildType.Release ? "Release" : "Debug",
                ["Platform"] = "x64",
                ["TargetFramework"] = "net9.0",
                // Publish settings
                ["RuntimeIdentifier"] = "win-x64",
                ["SelfContained"] = "true",

                // Trimming
                ["PublishTrimmed"] = "true",
                ["TrimMode"] = "link", // or 'full'

                // Output
                ["PublishDir"] = EditorPaths.Win32PublishFolderRoot + "\\",

                ["PublishSingleFile"] = "true",
                ["IncludeNativeLibrariesForSelfExtract"] = "true",
                ["EnableCompressionInSingleFile"] = "true",

                // Metadata
                ["Company"] = buildTypeSettings.Company,
                ["Product"] = buildTypeSettings.ApplicationName,
                ["Description"] = buildTypeSettings.Description,
                ["Authors"] = buildTypeSettings.Authors,

                ["AssemblyTitle"] = buildTypeSettings.Description,
                ["AssemblyDescription"] = buildTypeSettings.Description,

                ["AssemblyVersion"] = GetVersion(buildTypeSettings.Version),
                ["FileVersion"] = GetVersion(buildTypeSettings.Version),
                ["InformationalVersion"] = GetVersion(buildTypeSettings.Version),
            };

            var defaultConstants = "WINDOWS;WIN32;DESKTOP;";

            if (settings.Type == BuildType.Release)
            {
                props["DefineConstants"] = defaultConstants + "RELEASE";
                props["DebugType"] = "none";
                props["DebugSymbols"] = "false";
            }
            else
            {
                props["DefineConstants"] = defaultConstants + "DEBUG";
            }
            return props;
        }

        protected override void OnBuildSuccess()
        {
            var rootOutputFolder = EditorPaths.ShipWin32FolderRoot;

            var settings = GetBuildSettings<WindowsBuildSettings>(PlatformBuild.Windows);
            var buildTypeSettings = settings.GetCurrentBuildTypeSettings();
            if (!string.IsNullOrEmpty(buildTypeSettings.OutputPath))
            {
                rootOutputFolder = Paths.ClearPathSeparation(buildTypeSettings.OutputPath);
            }

            Directory.CreateDirectory(rootOutputFolder);

            // Rename executable
            var originalFileName = Path.Combine(EditorPaths.Win32PublishFolderRoot, $"{EditorPaths.DESKTOP_PROJECT_NAME}.exe");
            var newFileName = Path.Combine(EditorPaths.Win32PublishFolderRoot, $"{buildTypeSettings.ApplicationName}.exe");

            File.Move(originalFileName, newFileName, true);

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
            var miniaudioDllPath = Path.Combine(EditorPaths.EngineRoot, "Third", "SoundFlow", "runtimes", "win-x64", "native", miniaudioDllName);
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

        private string GetVersion(ivec3 version)
        {
            return $"{version.x}.{version.y}.{version.z}.{0}";
        }
    }
}
