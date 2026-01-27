using GameCooker;
using SharedTypes;
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
        internal static string DataRoot { get; }
        internal static string GameRoot { get; }
        public static string AndroidProjectRoot { get; }

        internal const string GAME_PROJECT_NAME = "Game";
        internal const string PROJECT_EXTENSION = ".csproj";
        internal const string GAME_PROJECT_FULL_NAME = GAME_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string GAME_BUILD_TYPE = "Debug";
        internal const string SHIP_FOLDER_NAME = "_Ship";

        internal const string ANDROID_PROJECT_NAME = "Entry_Android";
        internal const string ANDROID_PROJECT_FULL_NAME = ANDROID_PROJECT_NAME + PROJECT_EXTENSION;

        internal static string HookFolderRelativePath => $@"Library/Build/bin/{GAME_BUILD_TYPE}/Hook";
        internal static string NewGameDllRelativePath => $@"Library/Build/bin/{GAME_BUILD_TYPE}/{GAME_PROJECT_NAME}.dll";
        internal static string HookGameDllRelativePath => Path.Combine(HookFolderRelativePath, $"{GAME_PROJECT_NAME}.dll");

        internal static string GameBinFolderAbsolutePath => GetGameFolderAbsolutePath($@"Library/Build/bin/{GAME_BUILD_TYPE}");
        internal static string HookFolderAbsolutePath => GetGameFolderAbsolutePath(HookFolderRelativePath);
        internal static string CompiledGameDllAbsolutePath => GetGameFolderAbsolutePath(NewGameDllRelativePath);
        internal static string GameHookDLLAbsolutePath => GetGameFolderAbsolutePath(HookGameDllRelativePath);

        public static string ShipFolderRoot => Path.Combine(AppRoot, SHIP_FOLDER_NAME);
        public static string ShipAndroidFolderRoot => Path.Combine(AppRoot, SHIP_FOLDER_NAME, "android");
        public static string ShipWin32FolderRoot => Path.Combine(AppRoot, SHIP_FOLDER_NAME, "win32");
        public static string ShipMacOsFolderRoot => Path.Combine(AppRoot, SHIP_FOLDER_NAME, "osx");
        public static string ShipIOSFolderRoot => Path.Combine(AppRoot, SHIP_FOLDER_NAME, "ios");
        public static string ShipLinuxFolderRoot => Path.Combine(AppRoot, SHIP_FOLDER_NAME, "linux");

        static EditorPaths()
        {
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf("Editor")));

            AppRoot = root;
            DataRoot = Path.Combine(root, "Editor/Data");
            GameRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Game"));
            AndroidProjectRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Platforms/Android"));
        }

        public static string GetGameFolderAbsolutePath(string path)
        {
            return Paths.ClearPathSeparation(Path.Combine(GameRoot, path));
        }

        internal static string GetAbsolutePathSafe(string relativePath)
        {
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
    }
}
