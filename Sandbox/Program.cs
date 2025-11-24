using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using Engine;
using Engine.Layers;

using Game;
using GameCooker;
using SharedTypes;

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

            new GameProject().Initialize(new ProjectConfig() { ProjectFolderRoot = root });
            new AssetsCooker().CookAll(new CookOptions()
            {
                Type = CookingType.DevMode,
                AssetsFolderPath = Paths.GetAssetsFolderPath(),
                ExportFolderPath = Paths.GetAssetDatabaseFolder(),
                FileOptions = new CookFileOptions()
                {
                    CompressAllFiles = false,
                    CompressionLevel = 12,
                    EncryptAllFiles = false,
                }
            });
#endif
            new GFSEngine().Initialize<GameApplication>("GFS", 1024, 576).Run();

            _mutex.ReleaseMutex();
        }
    }
}