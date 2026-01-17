using SharedTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCooker
{
    internal class DevModeFilesCooker : AssetsCookerBase
    {
        private AssetsDatabaseInfo _database;

        public DevModeFilesCooker(AssetsDatabaseInfo database, Dictionary<AssetType, IAssetProcessor> assetProcessor) :
            base(assetProcessor)
        {
            _database = database;
        }

        internal override async Task CookAssetsAsync(CookFileOptions fileOptions, CookingPlatform platform, (string, AssetType)[] files, string outFolder)
        {
            _database.UpdatedAssets.Clear();

            foreach (var (filePath, assetType) in files)
            {
                if (filePath.EndsWith(Paths.ASSET_META_EXT_NAME))
                {
                    continue;
                }
                var metaPath = filePath + Paths.ASSET_META_EXT_NAME;

                var meta = AssetUtils.GetMeta(metaPath, assetType);

                AssetInfo assetInfo = null;

                bool constainsAssetInfo = _database.Assets.TryGetValue(meta.GUID, out assetInfo);

                bool isInLibrary = false;

                var binPath = Paths.CreateBinFilePath(outFolder, meta.GUID.ToString());

                if (meta != null && File.Exists(binPath))
                {
                    isInLibrary = true;
                }

                var latestWriteTime = File.GetLastWriteTime(filePath);
                var metaLatestWriteTime = File.GetLastWriteTime(metaPath);

                if (!constainsAssetInfo || latestWriteTime > assetInfo.LastWriteTime ||
                    !isInLibrary || metaLatestWriteTime > assetInfo.MetaWriteTime)
                {
                    var data = ProcessAsset(platform, assetType, meta, filePath);

                    var assetRelPath = Paths.GetRelativeAssetPath(filePath);
                    if (constainsAssetInfo)
                    {
                        Console.WriteLine("Updating asset file: " + filePath);
                        assetInfo = _database.Assets[meta.GUID];

                        assetInfo.LastWriteTime = latestWriteTime;
                        assetInfo.MetaWriteTime = metaLatestWriteTime;
                        assetInfo.Path = assetRelPath;
                        _database.UpdatedAssets.Add(meta.GUID);
                    }
                    else
                    {
                        Console.WriteLine("Importing asset file: " + filePath);

                        // Write meta
                        File.WriteAllText(metaPath, JsonConvert.SerializeObject(meta, Formatting.Indented));

                        _database.Assets.Add(meta.GUID, new AssetInfo()
                        {
                            Type = assetType,
                            Path = assetRelPath,
                            IsEncrypted = false,
                            IsCompressed = false,
                            LastWriteTime = latestWriteTime,
                            MetaWriteTime = File.GetLastWriteTime(metaPath)
                        });
                    }

                    // Write asset to library
                    File.WriteAllBytes(binPath, data ?? []);
                }
            }
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            // Clean the list of non existent assets.
            var currentIds = new HashSet<string>(files.Select(a => a.Item1), StringComparer.OrdinalIgnoreCase);
            foreach (var item in _database.Assets.ToList())
            {
                var absoluteAssetPath = Paths.GetAbsoluteAssetPath(item.Value.Path);
                if (!currentIds.Contains(absoluteAssetPath))
                {
                    _database.Assets.Remove(item.Key);

                    // Delete bin file in asset database.
                    var assetBinPath = Paths.CreateBinFilePath(Paths.GetAssetDatabaseFolder(), item.Key.ToString());
                    if (File.Exists(assetBinPath))
                    {
                        Console.WriteLine($"Deleting non existent asset from database: guid: '{item.Key}', Path: '{item.Value.Path}'");
                        File.Delete(assetBinPath);
                    }
                }
            }

            // Delete non used .mt file in 'Assets' folder
            var mtFiles = Directory.GetFiles(Paths.GetAssetsFolderPath(), "*.mt", SearchOption.AllDirectories);
            foreach (var metaPath in mtFiles)
            {
                var clearPath = Paths.ClearPathSeparation(metaPath);
                if (!currentIds.Contains(clearPath.Substring(0, clearPath.Length - Paths.ASSET_META_EXT_NAME.Length)) &&
                    File.Exists(clearPath))
                {
                    File.Delete(clearPath);
                    Console.WriteLine($"Deleting .mt file: {clearPath}");
                }
            }
            Console.ForegroundColor = prevColor;

            _database.TotalAssets = _database.Assets.Count;

            // Write asset database
            File.WriteAllText(Path.Combine(outFolder, Paths.ASSET_DATABASE_FILE_NAME), JsonConvert.SerializeObject(_database, Formatting.Indented));
        }
    }
}