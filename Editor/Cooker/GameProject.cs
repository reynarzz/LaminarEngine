using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class ProjectCreatedInfo
    {
        public string ProjectName = string.Empty;
        public string ProjectRootDirectory = string.Empty;
        public bool UseIntermediaryDirectory = false;

        public bool IsValidProjectData()
        {
            return !string.IsNullOrEmpty(ProjectName) && !string.IsNullOrEmpty(ProjectRootDirectory);
        }
    }

    public class GameProject
    {
        public static void Initialize(ProjectConfig config)
        {
            if (string.IsNullOrEmpty(config.ProjectFolderRoot) || !Directory.Exists(config.ProjectFolderRoot))
            {
                Console.WriteLine("Wrong root path");
                return;
            }
            InitializeProjectDirectories(config.ProjectFolderRoot);
        }

        private static void InitializeProjectDirectories(string rootFolder)
        {
            Paths.ProjectRootFolder = rootFolder;

            var directories = new string[]
            {
                Paths.GetAssetsFolderPath(),
                Paths.GetLibraryFolderPath(),
                Paths.GetProjectSettingsFolder(),
                Paths.GetAssetDatabaseFolder(),
            };
            foreach (var dir in directories)
            {
                Directory.CreateDirectory(dir);
            }
        }

        internal static void CreateDefaultProject(ProjectCreatedInfo info)
        {
            var root = info.ProjectRootDirectory;
            if (info.UseIntermediaryDirectory)
            {
                root = Path.Combine(root, info.ProjectName);

                Directory.CreateDirectory(root);
            }
            InitializeProjectDirectories(root);

            // TODO: Create default ProjectSettings.dat
        }
    }
}