using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    public static class CookerPaths
    {
        public static string AssetsPath { get; }
        public static string InternalAssetsPath { get; }
        public static string ShadersPath { get; }
        public static string CookerRoot { get; }
        public const string INTERNAL_ASSET_FOLDER_NAME = "__InternalAssets__";

        static CookerPaths()
        {
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            CookerRoot = Path.Combine(GetRootFolder(assemblyDir), "GameCooker");

            AssetsPath = Paths.ClearPathSeparation(Path.Combine(CookerRoot, "Assets"));
            InternalAssetsPath = Paths.ClearPathSeparation(Path.Combine(AssetsPath, INTERNAL_ASSET_FOLDER_NAME));
            ShadersPath = Paths.ClearPathSeparation(Path.Combine(InternalAssetsPath, "Shaders"));
        }

        private static string GetRootFolder(string startPath)
        {
            if (File.Exists(startPath + "/GameScratch.sln"))
            {
                return startPath;
            }

            return GetRootFolder(Directory.GetParent(startPath).FullName);
        }
    }
}
