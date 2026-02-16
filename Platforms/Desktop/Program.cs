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
            try
            {
                new GFSEngine(new WindowStandalone("GFS | By Reynardo Perez", 1280, 720, Color.Black),
                        new GameApplication(),
                        new InputStandAlonePlatform()).Run();
            }
            catch (Exception e)
            {
                File.WriteAllText("Error.txt", e.ToString());
            }
            _mutex.ReleaseMutex();
        }
    }
}