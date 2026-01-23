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
        internal const string GAME_PROJECT_NAME = "Game";
        internal const string PROJECT_EXTENSION = ".csproj";
        internal const string GAME_PROJECT_FULL_NAME = GAME_PROJECT_NAME + PROJECT_EXTENSION;
        internal const string GAME_BUILD_TYPE = "Debug";
        internal static string HookFolderRelativePath => $@"Library/Build/bin/{GAME_BUILD_TYPE}/Hook";
        internal static string NewGameDllRelativePath => $@"Library/Build/bin/{GAME_BUILD_TYPE}/{GAME_PROJECT_NAME}.dll";
        internal static string HookGameDllRelativePath => Path.Combine(HookFolderRelativePath, $"{GAME_PROJECT_NAME}.dll");

        internal static string GameBinFolderAbsolutePath => GetGameFolderAbsolutePath($@"Library/Build/bin/{GAME_BUILD_TYPE}");
        internal static string HookFolderAbsolutePath => GetGameFolderAbsolutePath(HookFolderRelativePath);
        internal static string CompiledGameDllAbsolutePath => GetGameFolderAbsolutePath(NewGameDllRelativePath);
        internal static string GameHookDLLAbsolutePath => GetGameFolderAbsolutePath(HookGameDllRelativePath);

       
        static EditorPaths()
        {
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf("Editor")));

            AppRoot = root;
            DataRoot = Path.Combine(root, "Editor/Data");
            GameRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Game"));
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
