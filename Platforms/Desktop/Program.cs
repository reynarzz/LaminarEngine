using Engine;
using Engine.Layers.Input;
using Game;

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
            var libsPath = Path.Combine(AppContext.BaseDirectory, "Data/Assemblies");

            string extension = string.Empty;

            if (OperatingSystem.IsWindows())
            {
                extension = ".dll";
            }
            else if (OperatingSystem.IsLinux())
            {
                extension = ".so";
            }
            else if (OperatingSystem.IsMacOS())
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
            try
            {
                new GFSEngine(new WindowStandalone("GFS | By Reynardo Perez", 1280, 720, Color.Black),
                        new GameApplication(),
                        new InputStandAlonePlatform()).Run();
            }
            catch (Exception e)
            {
#if DEBUG
                File.WriteAllText("Error.txt", e.ToString());
#endif
            }
            _mutex.ReleaseMutex();
        }
    }
}