using Editor.Utils;
using Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Cooker
{
    internal class DevModeFilesCooker : AssetsCookerBase
    {
        private AssetsDatabaseInfo _database; // make this private, this is a hack to make the tilemap importer work.
        public DevModeFilesCooker(AssetsDatabaseInfo database, Dictionary<AssetType, IAssetProcessor> assetProcessor) :
            base(assetProcessor)
        {
            _database = database;
        }

        // TODO: Split this function.
        internal override async Task<CookingResult> CookAssetsAsync(CookFileOptions fileOptions, CookingPlatform platform, (string, AssetType)[] files, string outFolder)
        {
            bool someAssetFailImport = false;
            Engine.Debug.Log("Dev asset cooking");
            _database.UpdatedAssets.Clear();
            int importCount = 0;
            var cookResult = new CookingResult();
            if (!Directory.Exists(outFolder))
            {
                Directory.CreateDirectory(outFolder);
            }
            foreach (var (filePath, assetType) in files)
            {
                if (filePath.EndsWith(Paths.ASSET_META_EXT_NAME))
                {
                    continue;
                }

                var meta = EditorAssetUtils.GetMetaFromAbsolutePath(filePath, assetType);

                bool containsAssetInfo = _database.Assets.TryGetValue(meta.GUID, out var assetInfo);

                bool isInLibrary = false;

                var binPath = Paths.CreateBinFilePath(outFolder, meta.GUID.ToString());

                if (meta != null && File.Exists(binPath))
                {
                    isInLibrary = true;
                }

                var metaPath = filePath + Paths.ASSET_META_EXT_NAME;
                var latestWriteTime = File.GetLastWriteTime(filePath);
                var metaLatestWriteTime = File.GetLastWriteTime(metaPath);
                var assetExists = File.Exists(EditorPaths.GetAbsolutePathSafe(assetInfo.Path));

                if (!containsAssetInfo || latestWriteTime > assetInfo.LastWriteTime ||
                   !isInLibrary || metaLatestWriteTime > assetInfo.MetaWriteTime || (!assetExists && containsAssetInfo))
                {
                    using var fileStream = File.OpenRead(filePath);
                    using var reader = new BinaryReader(fileStream);

                    var data = ProcessAsset(platform, assetType, meta, reader);

                    if (data.IsSuccess)
                    {
                        importCount++;
                    }
                    else
                    {
                        someAssetFailImport = true;
                    }
                    var assetRelPath = Paths.GetRelativeAssetPath(filePath);
                    if (containsAssetInfo)
                    {
                        Console.WriteLine("Updating asset file: " + filePath);
                        assetInfo = _database.Assets[meta.GUID];

                        assetInfo.LastWriteTime = latestWriteTime;
                        assetInfo.MetaWriteTime = metaLatestWriteTime;
                        assetInfo.Path = assetRelPath;
                        _database.Assets[meta.GUID] = assetInfo;

                        _database.UpdatedAssets.Add(meta.GUID);
                    }
                    else
                    {
                        Console.WriteLine("Importing asset file: " + filePath);

                        // Write meta
                        await File.WriteAllTextAsync(metaPath, EditorJsonUtils.Serialize(meta));

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
                    await File.WriteAllBytesAsync(binPath, data.Data ?? []);
                }
            }

            // Clear orphan assets and metas.
            ClearInvalidAssets(files.Select(a => a.Item1).ToList());

            cookResult.ChangedCount = Math.Abs(_database.TotalAssets - _database.Assets.Count);
            _database.TotalAssets = _database.Assets.Count;
            Debug.Log(cookResult.ChangedCount);
            try
            {
                // Write asset database
                await File.WriteAllTextAsync(Path.Combine(outFolder, Paths.ASSET_DATABASE_FILE_NAME), JsonConvert.SerializeObject(_database, Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine("Check Permission: Dev mode files cooker.");
            }
            cookResult.IsSuccess = !someAssetFailImport;
            return cookResult;
        }

        private void ClearInvalidAssets(List<string> files)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            // Clean the list of non existent assets.
            var currentIds = new HashSet<string>(files, StringComparer.OrdinalIgnoreCase);
            foreach (var (refId, assetInfo) in _database.Assets.ToList())
            {
                string absoluteAssetPath = null;

                if (!assetInfo.Path.StartsWith(EditorPaths.CookerPaths.INTERNAL_ASSET_FOLDER_NAME))
                {
                    absoluteAssetPath = Paths.GetAbsoluteAssetPath(assetInfo.Path);
                }
                else
                {
                    absoluteAssetPath = Paths.ClearPathSeparation(Path.Combine(EditorPaths.CookerPaths.AssetsPath, assetInfo.Path));
                }

                if (!currentIds.Contains(absoluteAssetPath))
                {
                    var assetBinPath = Paths.CreateBinFilePath(Paths.GetAssetDatabaseFolder(), refId.ToString());

                    if (!IsMove(refId, assetInfo.Path))
                    {
                        _database.Assets.Remove(refId);

                        // Delete bin file in asset database.
                        if (File.Exists(assetBinPath))
                        {
                            Console.WriteLine($"Deleting non existent asset from database: guid: '{refId}', Path: '{assetInfo.Path}'");
                            File.Delete(assetBinPath);
                        }
                    }
                }
            }

            // Delete non used .mt file in 'Assets' folder
            var mtFiles = Directory.GetFiles(Paths.GetAssetsFolderPath(), "*.mt", SearchOption.AllDirectories);//.ToList();
                                                                                                               // mtFiles.AddRange(Directory.GetFiles(CookerPaths.InternalAssetsPath, "*.mt", SearchOption.AllDirectories));

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
        }

        private bool IsMove(Guid refId, string filePath)
        {
            if(_database.Assets.TryGetValue(refId, out var currentAssetPath))
            {
                return !currentAssetPath.Equals(filePath);
            }

            return false;
        }
    }
}