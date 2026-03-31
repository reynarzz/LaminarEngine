using Editor.Cooker;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal static class EditorPaths
    {
        internal static string AppRoot { get; }
        public static string EditorRoot { get; }
        internal static string EditorDataRoot { get; }
        internal static string GameRoot { get; set; }
        internal static string EngineRoot { get; }
        public static string AndroidProjectRoot { get; }
        public static string DesktopProjectRoot { get; }

        internal const string EDITOR_NAME = Paths.ENGINE_NAME + " Editor";
        internal const string EDITOR_DIRTY_NAME = EDITOR_NAME + "*";
        internal const string PROJECT_EXTENSION = ".csproj";
        internal const string EDITOR_DATA_EXTENSION = ".dat";
        internal const string SCENE_FILE_EXTENSION = ".scene";
        internal const string ENGINE_PROJECT_NAME = "Engine";
        internal const string ENGINE_PROJECT_FULL_NAME = ENGINE_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string XML_EXTENSION = ".xml";

        internal const string GENERATED_PROJECT_NAME = "Generated";
        internal const string GENERATED_LINKER_RD_NAME = "rd";
        internal const string GENERATED_PROJECT_FULL_NAME = GENERATED_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string GENERATED_LINKER_RD_FULL_NAME = GENERATED_LINKER_RD_NAME + XML_EXTENSION;

        internal const string GAME_BUILD_TYPE = "Debug";
        internal const string SHIP_DEFAULT_FOLDER_NAME = "_Ship";
        internal const string EDITOR_RESOURCES_DIRECTORY_NAME = "Resources";

        internal const string ANDROID_PROJECT_NAME = "Entry_Android";
        internal const string DESKTOP_PROJECT_NAME = "Entry_Desktop";
        internal const string GAME_PROJECT_NAME = "Game";
        internal const string ANDROID_PROJECT_FULL_NAME = ANDROID_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string DESKTOP_PROJECT_FULL_NAME = DESKTOP_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string GAME_PROJECT_FULL_NAME = GAME_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string GAME_PROJECT_TEMPLATE_FILE_NAME = "GameProject_TEMPLATE.txt";
        internal const string GENERATED_PROJECT_TEMPLATE_FILE_NAME = "GeneratedProject_TEMPLATE.txt";

        internal const string WIN32_DATA_SHIP_FOLDER_NAME = "Data";

        internal const string BUILD_SETTINGS_NAME = "BuildSettings";
        internal const string PROJECT_SETTINGS_NAME = "ProjectSettings";
        internal const string EDITOR_SETTINGS_NAME = "EditorSettings";

        internal const string PROJECT_SETTINGS_DAT_FULL_NAME = PROJECT_SETTINGS_NAME + EDITOR_DATA_EXTENSION;

        internal const string LIBRARY_FOLDER_NAME = "Library";
        internal const string BUILD_FOLDER_NAME = "Build";
        internal const string GENERATED_FOLDER_NAME = "Generated";

        internal const string EDITOR_TEMPLATES_FOLDER_NAME = "Templates";
        internal static string BuildFolderRelativePath => Path.Combine(LIBRARY_FOLDER_NAME, BUILD_FOLDER_NAME);
        internal static string HookFolderRelativePath => $@"{BuildFolderRelativePath}/bin/{GAME_BUILD_TYPE}/Hook";
        internal static string NewGameDllRelativePath => $@"{BuildFolderRelativePath}/bin/{GAME_BUILD_TYPE}/{GameCsProjName}.dll";
        internal static string HookGameDllRelativePath => Path.Combine(HookFolderRelativePath, $"{GameCsProjName}.dll");

        internal static string GameCsProjName { get; set; }
        internal static string GameBinFolderAbsolutePath => GetGameFolderAbsolutePath($@"{BuildFolderRelativePath}/bin/{GAME_BUILD_TYPE}");
        internal static string HookFolderAbsolutePath => GetGameFolderAbsolutePath(HookFolderRelativePath);
        internal static string CompiledGameDllAbsolutePath => GetGameFolderAbsolutePath(NewGameDllRelativePath);
        internal static string GameHookDLLAbsolutePath => GetGameFolderAbsolutePath(HookGameDllRelativePath);
        internal static string GameBuildFolderAbsolutePath => GetGameFolderAbsolutePath(BuildFolderRelativePath);
        internal static string GameProjectAbsolutePath => Path.Combine(GameRoot, GameCSProjFullName);
        internal static string GameCSProjFullName => GameCsProjName + PROJECT_EXTENSION;
        internal static string GameGeneratedProjectFolderAbsolutePath => GetGameFolderAbsolutePath($"{BuildFolderRelativePath}/{GENERATED_FOLDER_NAME}");
        internal static string GameGeneratedProjectCsProjFileFullPath => Path.Combine(GameGeneratedProjectFolderAbsolutePath, GENERATED_PROJECT_FULL_NAME);
        internal static string GameGeneratedLinkerRDFileFullPath => Path.Combine(GameGeneratedProjectFolderAbsolutePath, GENERATED_LINKER_RD_FULL_NAME);

        internal static string DesktopCsProjFullPath => Path.Combine(DesktopProjectRoot, DESKTOP_PROJECT_FULL_NAME);
        public static string ShipFolderRoot => Path.Combine(GameRoot, SHIP_DEFAULT_FOLDER_NAME);
        public static string AndroidShipFolderRoot => Path.Combine(ShipFolderRoot, "android");
        public static string AndroidProjectAssetsFolderRoot => Path.Combine(AndroidProjectRoot, "Assets");
        public static string AndroidPublishFolderRoot => Path.Combine(AndroidProjectRoot, "bin", "Publish");
        public static string DesktopPublishFolderRoot => Path.Combine(DesktopProjectRoot, "bin", "Publish");
        public static string Win32PublishFolderRoot => Path.Combine(DesktopPublishFolderRoot, "win32");

        public static string ShipWin32FolderRoot => Path.Combine(ShipFolderRoot, "win32");
        public static string Win32ShipGameDataFolderRoot => Path.Combine(ShipWin32FolderRoot, "Data");

        public static string ShipMacOsFolderRoot => Path.Combine(ShipFolderRoot, "osx");
        public static string ShipIOSFolderRoot => Path.Combine(ShipFolderRoot, "ios");
        public static string ShipLinuxFolderRoot => Path.Combine(ShipFolderRoot, "linux");

        public static string EngineNativesFolderRoot => Path.Combine(AppRoot, "Engine", "Third", "Native", "bin");
        public static string EngineWin32NativesFolderRoot => Path.Combine(EngineNativesFolderRoot, "win");
        public static string EngineCsProjFullPath => Paths.ClearPathSeparation(Path.Combine(EngineRoot, ENGINE_PROJECT_FULL_NAME));

        public static readonly string ConfigDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Paths.ENGINE_NAME);
        public static string EditorTemplatesFolderFullPath => Path.Combine(EditorResourceFullPath, EDITOR_TEMPLATES_FOLDER_NAME);
        public static string EditorResourceFullPath { get; }

        static EditorPaths()
        {
            var root = Paths.ClearPathSeparation(GetRootFolder(AppContext.BaseDirectory) + Path.DirectorySeparatorChar);

            AppRoot = Paths.ClearPathSeparation(root);
            EditorRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Editor"));
            EditorDataRoot = Paths.ClearPathSeparation(Path.Combine(EditorRoot, "Data"));
            AndroidProjectRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Platforms", "Android"));
            DesktopProjectRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Platforms", "Desktop"));
            EngineRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Engine"));

            EditorResourceFullPath = Paths.ClearPathSeparation(Path.Combine(EditorDataRoot, EDITOR_RESOURCES_DIRECTORY_NAME));
        }

        private static string GetRootFolder(string startPath)
        {
            if (File.Exists($"{startPath}/{Paths.ENGINE_NAME}.sln"))
            {
                return startPath;
            }

            return GetRootFolder(Directory.GetParent(startPath).FullName);
        }

        public static string GetGameFolderAbsolutePath(string path)
        {
            return Paths.ClearPathSeparation(Path.Combine(GameRoot, path));
        }

        internal static string GetAbsolutePathSafe(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                Debug.Error("Path is empty");
                return string.Empty;
            }
            string absoluteAssetPath = null;
            if (!relativePath.StartsWith(CookerPaths.INTERNAL_ASSET_FOLDER_NAME))
            {
                absoluteAssetPath = Paths.GetAbsoluteAssetPath(relativePath);
            }
            else
            {
                absoluteAssetPath = Paths.ClearPathSeparation(Path.Combine(CookerPaths.AssetsPath, relativePath));
            }
            return absoluteAssetPath;
        }

        internal static string GetRelativeAssetPathSafe(string absolutePath)
        {
            absolutePath = Paths.ClearPathSeparation(absolutePath);
            bool GetRelative(string root, out string value)
            {
                value = string.Empty;
                var idx = absolutePath.IndexOf(root);
                if (idx <= 0)
                {
                    return false;
                }

                value = absolutePath.Substring(idx);

                return true;
            }

            if (GetRelative(CookerPaths.INTERNAL_ASSET_FOLDER_NAME, out var internalAssetsPath))
            {
                return internalAssetsPath;
            }
            else if (GetRelative(Paths.ASSETS_FOLDER_NAME, out var assetsPath))
            {
                return assetsPath;
            }

            return string.Empty;
        }


        public static class CookerPaths
        {
            public static string AssetsPath { get; }
            public static string InternalAssetsPath { get; }
            public static string ShadersPath { get; }
            public static string CookerRoot { get; }
            public const string INTERNAL_ASSET_FOLDER_NAME = "__InternalAssets__";

            static CookerPaths()
            {
                CookerRoot = Path.Combine(EditorRoot, "Cooker");
                AssetsPath = Paths.ClearPathSeparation(Path.Combine(CookerRoot, "Assets"));
                InternalAssetsPath = Paths.ClearPathSeparation(Path.Combine(AssetsPath, INTERNAL_ASSET_FOLDER_NAME));
                ShadersPath = Paths.ClearPathSeparation(Path.Combine(InternalAssetsPath, "Shaders"));
            }
        }
    }
}
