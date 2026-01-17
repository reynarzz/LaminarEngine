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
        public static string AppRoot { get; }
        public static string DataRoot { get; }
        public static string GameRoot { get; }
        public const string GAME_PROJECT_NAME = "Game";
        public const string PROJECT_EXTENSION = ".csproj";
        public const string GAME_PROJECT_FULL_NAME = GAME_PROJECT_NAME + PROJECT_EXTENSION;

        static EditorPaths()
        {
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf("Editor")));

            AppRoot = root;
            DataRoot = Path.Combine(root, "Editor/Data");
            GameRoot = Paths.ClearPathSeparation(Path.Combine(AppRoot, "Game"));
        }
    }
}
