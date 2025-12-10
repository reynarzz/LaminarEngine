using Engine;
using Game;
using SharedTypes;
using System.Runtime.InteropServices;

namespace Sandbox
{
    internal class Program
    {
        private static readonly Mutex _mutex = new Mutex(false, "Global\\SandboxApp_Game");

        private static void Main()
        {
            if (!_mutex.WaitOne(0, false))
            {
                return;
            }
#if RELEASE
            var libsPath = Path.Combine(AppContext.BaseDirectory, "Data/Assemblies");

            foreach (var dll in Directory.GetFiles(libsPath, "*.dll"))
            {
                try
                {
                    NativeLibrary.Load(dll);
                }
                catch { }
            }
#else

            // This will import all the assets without using the GUI tool. Useful for running the project in debug mode.
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf(Paths.SANDBOX_FOLDER_NAME)), Paths.GAME_FOLDER_NAME);

            new GameCooker.GameProject().Initialize(new GameCooker.ProjectConfig() { ProjectFolderRoot = root });
            var releaseAssetsPath = Paths.GetLibraryFolderPath() + "/_ReleaseAssetsList.txt";
            var releaseAssetsList = default(string[]);
            if (File.Exists(releaseAssetsPath))
            {
                releaseAssetsList = File.ReadAllText(releaseAssetsPath)?.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            }
            new GameCooker.AssetsCooker().CookAll(new GameCooker.CookOptions()
            {
                Type = GameCooker.CookingType.ReleaseMode,
                AssetsFolderPath = Paths.GetAssetsFolderPath(),
                ExportFolderPath = Paths.GetAssetDatabaseFolder(),
                FileOptions = new GameCooker.CookFileOptions()
                {
                    CompressAllFiles = true,
                    CompressionLevel = 12,
                    EncryptAllFiles = true,
                },
                MatchingFiles = releaseAssetsList
            });
#endif
            new GFSEngine().Initialize<GameApplication>("GFS | By Reynardo Perez", 1024, 576).Run();

            _mutex.ReleaseMutex();
        }
    }
}