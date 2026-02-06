using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
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

        private static void InitializeProjectDirectories(string projectRoot)
        {
            Paths.ProjectRootFolder = projectRoot;
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
    }
}