using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class Paths
    {
        public const string ENGINE_NAME = "Laminar";
        public const string LIBRARY_FOLDER_NAME = "Library";
        public const string ASSETS_FOLDER_NAME = "Assets";
        public const string GAME_FOLDER_NAME = "Game";
        public const string SHIP_ASSETS_LIST_FILE_NAME = "ShipAssets.txt";
        public const string PROJECT_CONFIG_FOLDER_NAME = "ProjectSettings";

        public const string ASSET_DATABASE_FOLDER_NAME = "AssetsDatabase";
        public const string ASSET_DATABASE_FILE_NAME = "AssetsDatabase.txt";
        public const string ASSET_DATABASE_BINARY_EXT_NAME = ".bin";
        public const string ASSET_META_EXT_NAME = ".mt";
        public const string ASSET_BUILD_DATA_EXT_NAME = ".pak";
        public const string ASSET_BUILD_DATA_FILE_NAME = "package"; 
        public const string ASSET_BUILD_DATA_FULL_FILE_NAME = ASSET_BUILD_DATA_FILE_NAME + ASSET_BUILD_DATA_EXT_NAME; 
        public const string ASSET_BUILD_DATA_FILE_META_NAME = ASSET_BUILD_DATA_FILE_NAME + ASSET_META_EXT_NAME; 
        public const string RELEASE_BUILD_DATA_FOLDER_NAME = "Data";
        internal const string SHIP_LIBRARIES_FOLDER_NAME = "Internal";

        private static string _projectRootFolder;
        public static string ProjectRootFolder
        {
            get => _projectRootFolder;
            internal set
            {
                if (!Path.IsPathRooted(value))
                {
                    throw new ArgumentException("ProjectRootFolder must be an absolute path.");
                }

                _projectRootFolder = Path.GetFullPath(value.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            }
        }

        public static string GetReleaseDataFolder(bool isRelativePath = false)
        {
            return ClearPathSeparation(Path.Join(AppContext.BaseDirectory, RELEASE_BUILD_DATA_FOLDER_NAME));
        }

        public static string GetAssetDatabaseFolder(bool isRelativePath = false)
        {
            return ClearPathSeparation(Path.Join(GetAbsolutePathFlag(isRelativePath), GetLibraryFolderPath(true), ASSET_DATABASE_FOLDER_NAME));
        }
        public static string GetProjectSettingsFolder(bool isRelativePath = false)
        {
            return ClearPathSeparation(Path.Join(GetAbsolutePathFlag(isRelativePath), PROJECT_CONFIG_FOLDER_NAME));
        }

        public static string GetShipAssetsFilePath(bool isRelativePath = false)
        {
            return ClearPathSeparation(Path.Combine(GetProjectSettingsFolder(), SHIP_ASSETS_LIST_FILE_NAME));
        }

        public static string GetAssetDatabaseFilePath(bool isRelativePath = false)
        {
            return ClearPathSeparation(Path.Join(GetAbsolutePathFlag(isRelativePath), GetAssetDatabaseFolder(true), ASSET_DATABASE_FILE_NAME));
        }

        public static string GetLibraryFolderPath(bool isRelativePath = false)
        {
            return ClearPathSeparation(Path.Join(GetAbsolutePathFlag(isRelativePath), LIBRARY_FOLDER_NAME));
        }

        public static string GetAssetsFolderPath(bool isRelativePath = false)
        {
            return ClearPathSeparation(Path.Join(GetAbsolutePathFlag(isRelativePath), ASSETS_FOLDER_NAME));
        }

        public static string CreateBinFilePath(string folderPath, string guid, bool isRelativePath = false)
        {
            return ClearPathSeparation(Path.Join(folderPath, guid + ASSET_DATABASE_BINARY_EXT_NAME));
        }

        public static string GetRelativeAssetPath(string absoluteAssetPath, string assetFolderName = ASSETS_FOLDER_NAME)
        {
            return ClearPathSeparation(absoluteAssetPath.Substring(absoluteAssetPath.IndexOf(assetFolderName) + assetFolderName.Length + 1));
        }

        public static string GetAbsoluteAssetPath(string relativeAssetPath)
        {
            return ClearPathSeparation(Path.Combine(ProjectRootFolder, ASSETS_FOLDER_NAME, relativeAssetPath));
        }

        public static string ClearPathSeparation(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            return path.Replace('\\', '/');
        }
        private static string GetAbsolutePathFlag(bool isRelativePath)
        {
            return isRelativePath ? null : ProjectRootFolder;
        }
    }

}
