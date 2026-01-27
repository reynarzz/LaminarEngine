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
            
        private readonly string[] _buildFilesExt = { ".exe",  }; // copy the dlls to data folder.
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
                ["TrimMode"] = "full", // link
                                                              
                // Output
                ["PublishDir"] = EditorPaths.Win32PublishFolderRoot + "\\",

                ["PublishSingleFile"] = "true",
                ["IncludeNativeLibrariesForSelfExtract"] = "true",
                ["EnableCompressionInSingleFile"] = "true",
            };
        }

        protected override void OnBuildSuccess()
        {
            Directory.CreateDirectory(EditorPaths.ShipWin32FolderRoot);

            foreach (var file in Directory.EnumerateFiles(EditorPaths.Win32PublishFolderRoot))
            {
                if (_buildFilesExt.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase))
                {
                    File.Copy(file, Path.Combine(EditorPaths.ShipWin32FolderRoot, "Game"), overwrite: true);
                }
            }
        }

        protected override string GetCSProjPath()
        {
            return Path.Combine(EditorPaths.DesktopProjectRoot, EditorPaths.DESKTOP_PROJECT_FULL_NAME);
        }

        protected override string[] GetTargetsToBuild()
        {
            return _targets;
        }

        public override bool ShouldBuild()
        {
            return true;
        }
    }
}
