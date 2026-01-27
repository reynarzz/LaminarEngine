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
        public WindowsProjectBuildStage() : base(new BuildLogger()
        {
            DebugStatus = true
        })
        { }

        protected override Dictionary<string, string> GetBuildProperties()
        {
            return new()
            {
                ["Configuration"] = "Release",
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
                ["Company"] = "My Company LLC",
                ["Product"] = "My Game",
                ["Description"] = "My Game Description",
                ["Authors"] = "My Company LLC",

                ["AssemblyTitle"] = "My Game",
                ["AssemblyDescription"] = "My Game Description",

                ["AssemblyVersion"] = "1.2.3.0",
                ["FileVersion"] = "1.2.3.0",
                ["InformationalVersion"] = "1.2.3"
            };
        }

        protected override void OnBuildSuccess()
        {
            Directory.CreateDirectory(EditorPaths.ShipWin32FolderRoot);

            // Rename executable
            var appName = "Game";
            File.Move(Path.Combine(EditorPaths.Win32PublishFolderRoot, EditorPaths.DESKTOP_PROJECT_NAME + ".exe"),
                      Path.Combine(EditorPaths.Win32PublishFolderRoot, $"{appName}.exe"), true);

            // Copy build files
            foreach (var file in Directory.EnumerateFiles(EditorPaths.Win32PublishFolderRoot))
            {
                if (_buildFilesExt.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                {
                    File.Copy(file, Path.Combine(EditorPaths.ShipWin32FolderRoot, Path.GetFileName(file)), overwrite: true);
                }
            }

            // Copy assemblies: TODO: create a list of dependencies.
            var assembliesFolder = Path.Combine(EditorPaths.Win32ShipGameDataFolderRoot, "Assemblies");
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
