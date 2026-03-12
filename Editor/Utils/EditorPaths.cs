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
        internal static string DataRoot { get; }
        internal static string GameRoot { get; }
        internal static string EngineRoot { get; }
        public static string AndroidProjectRoot { get; }
        public static string DesktopProjectRoot { get; }

        internal const string GAME_PROJECT_NAME = "Game";
        internal const string PROJECT_EXTENSION = ".csproj";
        internal const string EDITOR_DATA_EXTENSION = ".dat";

        internal const string GAME_PROJECT_FULL_NAME = GAME_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string GAME_BUILD_TYPE = "Debug";
        internal const string SHIP_FOLDER_NAME = "_Ship";

        internal const string ANDROID_PROJECT_NAME = "Entry_Android";
        internal const string DESKTOP_PROJECT_NAME = "Entry_Desktop";
        internal const string ANDROID_PROJECT_FULL_NAME = ANDROID_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string DESKTOP_PROJECT_FULL_NAME = DESKTOP_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string WIN32_DATA_SHIP_FOLDER_NAME = "Data";

        internal static string HookFolderRelativePath => $@"Library/Build/bin/{GAME_BUILD_TYPE}/Hook";
        internal static string NewGameDllRelativePath => $@"Library/Build/bin/{GAME_BUILD_TYPE}/{GAME_PROJECT_NAME}.dll";
        internal static string HookGameDllRelativePath => Path.Combine(HookFolderRelativePath, $"{GAME_PROJECT_NAME}.dll");

        internal static string GameBinFolderAbsolutePath => GetGameFolderAbsolutePath($@"Library/Build/bin/{GAME_BUILD_TYPE}");
        internal static string HookFolderAbsolutePath => GetGameFolderAbsolutePath(HookFolderRelativePath);
        internal static string CompiledGameDllAbsolutePath => GetGameFolderAbsolutePath(NewGameDllRelativePath);
        internal static string GameHookDLLAbsolutePath => GetGameFolderAbsolutePath(HookGameDllRelativePath);
        internal static string GameProjectAbsolutePath => Path.Combine(GameRoot, GAME_PROJECT_FULL_NAME);


        public static string ShipFolderRoot => Path.Combine(AppRoot, SHIP_FOLDER_NAME);
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

        static EditorPaths()
        {
            var root = Paths.ClearPathSeparation(GetRootFolder(AppContext.BaseDirectory) + Path.DirectorySeparatorChar);

            AppRoot = Paths.ClearPathSeparation(root);
            EditorRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Editor"));
            DataRoot = Paths.ClearPathSeparation(Path.Combine(EditorRoot, "Data"));
            GameRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, Paths.GAME_FOLDER_NAME));
            AndroidProjectRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Platforms", "Android"));
            DesktopProjectRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Platforms", "Desktop"));
            EngineRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Engine"));
        }

        private static string GetRootFolder(string startPath)
        {
            if (File.Exists(startPath + "/GameScratch.sln"))
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
