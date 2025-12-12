using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    public static class CLIEntryPoint
    {
        private static readonly Mutex _mutex = new Mutex(false, "Global\\CLI_GAMECOOKER");

        public static void Main(string[] args)
        {
            if (!_mutex.WaitOne(0, false))
            {
                return;
            }

            // AssetsFolderPath outPath cookTypeIndex platformIndex
            Console.WriteLine("Asset cooker");
            Console.WriteLine("Arg 0: entry folder Path");
            Console.WriteLine("Arg 1: output folder Path");
            Console.WriteLine("Arg 2: 0 = Monolith, 1 = Separated files");
            Console.WriteLine("Arg 3: platform");

            Console.WriteLine("Ex: Path/To/OutputFolder 1");

            var matchingAssetsPath = args.ElementAtOrDefault(4); 
            var releaseAssetsList = default(string[]);

            if (File.Exists(matchingAssetsPath))
            {
                releaseAssetsList = File.ReadAllText(matchingAssetsPath)?.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            }
            try
            {
                new AssetsCooker().CookAll(new CookOptions()
                {
                    AssetsFolderPath = args[0],
                    ExportFolderPath = args[1],
                    Type = (CookingType)int.Parse(args[2]),
                    Platform = (CookingPlatform)int.Parse(args[3]),
                    FileOptions = new CookFileOptions()
                    {
                        CompressAllFiles = true,
                        EncryptAllFiles = false,
                        EncryptFilesPath = false
                    },
                    MatchingFiles = releaseAssetsList
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            _mutex.ReleaseMutex();
        }
    }
}