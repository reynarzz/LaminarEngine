using Engine;
using Game;
using SharedTypes;

namespace Sandbox
{
    internal class Program
    {
        private static readonly Mutex _mutex = new Mutex(false, "Global\\SandboxApp_Game");
        private const string PLATFORMS_FOLDER_NAME = "Platforms";

        private static void Main()
        {
            if (!_mutex.WaitOne(0, false))
            {
                return;
            }
#if RELEASE
            var libsPath = Path.Combine(AppContext.BaseDirectory, "Data/Assemblies");
       
            string extension = string.Empty;

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows))
            {
                extension = ".dll";
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                     System.Runtime.InteropServices.OSPlatform.Linux))
            {
                extension = ".so";
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                     System.Runtime.InteropServices.OSPlatform.OSX))
            {
                extension = ".dylib";
            }
            else
            {
                throw new Exception("Unknown platform");
            }
            try
            {
                foreach (var dll in Directory.GetFiles(libsPath, "*" + extension))
                {
                    System.Runtime.InteropServices.NativeLibrary.Load(dll);
                }
            }
            catch { }
#else

            // This will import all the assets without using the GUI tool. Useful for running the project in debug mode.
            var assemblyDir = Paths.ClearPathSeparation(Path.GetDirectoryName(AppContext.BaseDirectory)!);
            var root = Path.Combine(assemblyDir.Substring(0, assemblyDir.LastIndexOf(PLATFORMS_FOLDER_NAME)), Paths.GAME_FOLDER_NAME);

            new GameCooker.GameProject().Initialize(new GameCooker.ProjectConfig() { ProjectFolderRoot = root });
            var releaseAssetsList = default(string[]);
            if (File.Exists(Paths.GetShipAssetsFilePath()))
            {
                releaseAssetsList = File.ReadAllText(Paths.GetShipAssetsFilePath())?.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            }
            new GameCooker.AssetsCooker().CookAll(new GameCooker.CookOptions()
            {
                Type = GameCooker.CookingType.DevMode,
                Platform = GameCooker.CookingPlatform.Windows,
                AssetsFolderPath = Paths.GetAssetsFolderPath(),
                ExportFolderPath = Paths.GetAssetDatabaseFolder(),
                FileOptions = new GameCooker.CookFileOptions()
                {
                    CompressAllFiles = false,
                    CompressionLevel = 12,
                    EncryptAllFiles = false,
                },
                MatchingFiles = releaseAssetsList
            });
#endif
            new GFSEngine(new Window("GFS | By Reynardo Perez", 1024, 576, Color.Black), new GameApplication()).Run();

            _mutex.ReleaseMutex();
        }
    }
}