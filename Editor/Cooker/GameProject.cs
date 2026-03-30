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

            GenerateGameProject(rootFolder);
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

            GenerateGameProject(root);
            // TODO: Create default ProjectSettings.dat
        }

        private const string ENGINE_PATH_GAME_PROJECT_TAG = "$__ENGINE_FULL_PATH__";

        private static void GenerateGameProject(string root)
        {
            var template = File.ReadAllText(Path.Combine(EditorPaths.EditorResourceFullPath, "GameProjectTemplate.txt"));
            var gamecsProj = template.Replace(ENGINE_PATH_GAME_PROJECT_TAG, Path.GetFullPath(EditorPaths.EngineCsProjFullPath));
            File.WriteAllText(Path.Combine(root, EditorPaths.GAME_PROJECT_FULL_NAME), gamecsProj);
        }
    }
}