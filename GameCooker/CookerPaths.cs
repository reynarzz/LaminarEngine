using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    internal static class CookerPaths
    {
        public static string AssetsPath { get;  }
        public static string ShadersPath { get;  }
        public static string CookerRoot { get;  }

        static CookerPaths()
        {
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            CookerRoot = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf("Editor")), "GameCooker");

            AssetsPath = Paths.ClearPathSeparation(Path.Combine(CookerRoot, "Assets"));
            ShadersPath = Paths.ClearPathSeparation(Path.Combine(AssetsPath, "Shaders"));
        }
    }
}
