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
        private const string ENGINE_PATH_GAME_PROJECT_TAG = "$__ENGINE_FULL_PATH__";

        public static void Initialize(ProjectConfig config)
        {
            if (string.IsNullOrEmpty(config.ProjectFolderRoot) || !Directory.Exists(config.ProjectFolderRoot))
            {
                Console.WriteLine("Wrong root path");
                return;
            }
            InitializeProjectDirectories(config.ProjectFolderRoot);
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

        private static void GenerateGameProject(string root)
        {
            // Generate Game.csproj
           
            var gameCsProjTemplate = LoadTemplate(EditorPaths.GAME_PROJECT_TEMPLATE_FILE_NAME);
            var gamecsProj = gameCsProjTemplate.Replace(ENGINE_PATH_GAME_PROJECT_TAG, Path.GetFullPath(EditorPaths.EngineCsProjFullPath));
            File.WriteAllText(Path.Combine(root, EditorPaths.GAME_PROJECT_FULL_NAME), gamecsProj);

            // Generate GameApplication.cs
            var gameAppTemplate = LoadTemplate(EditorPaths.GAME_APPLICATION_TEMPLATE_CS_FILE_NAME);
            File.WriteAllText(Path.Combine(root, Paths.ASSETS_FOLDER_NAME, EditorPaths.GAME_APPLICATION_CS_FILE_NAME), gameAppTemplate);


            string LoadTemplate(string templateRelFileName)
            {
                return File.ReadAllText(Path.Combine(EditorPaths.EditorResourceFullPath, templateRelFileName));
            }
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
    }
}