using Engine.Utils;
using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal class AssetDatabase
    {
        private readonly Dictionary<AssetType, IAssetBuilder> _assetbuilder;
        private readonly AssetDatabaseCache _databaseCache;
        private readonly BiDictionary<Guid, string> _guidPathDict = new();
#if SHIP_BUILD
        private readonly Dictionary<AssetType, Dictionary<Guid, AssetInfo>> _assetsGuidByTypes = new();
#else
        private readonly Dictionary<AssetType, OrderedDictionary<Guid, AssetInfo>> _assetsGuidByTypes = new();
#endif

        private DiskBase _disk;
        internal DiskBase Disk => _disk;
        public AssetDatabase(Dictionary<AssetType, IAssetBuilder> assetBuilder)
        {
            _assetbuilder = assetBuilder;
            _databaseCache = new();
        }

        internal virtual void Initialize(DiskBase disk)
        {
            _disk = disk;
            _guidPathDict.Clear();
            _assetsGuidByTypes.Clear();

            foreach (var (guid, info) in _disk.AssetDatabaseInfo.Assets)
            {
                _guidPathDict.Add(guid, info.Path);

                if (!_assetsGuidByTypes.TryGetValue(info.Type, out var guidList))
                {
#if SHIP_BUILD
                    guidList = new Dictionary<Guid, AssetInfo>();
#else
                    guidList = new OrderedDictionary<Guid, AssetInfo>();
#endif
                    _assetsGuidByTypes[info.Type] = guidList;
                }

                guidList.Add(guid, info);
            }
        }

        internal IDictionary<Guid, AssetInfo> GetAssetsInfoByType(AssetType assetType)
        {
            if (_assetsGuidByTypes.TryGetValue(assetType, out var guidList))
            {
                return guidList;
            }

            return null;
        }
        internal bool ExistsAsset(Guid id)
        {
            return _disk.AssetDatabaseInfo.Assets.ContainsKey(id);
        }
        internal AssetInfo GetAssetInfo(Guid id)
        {
            if (_disk.AssetDatabaseInfo.Assets.TryGetValue(id, out var info))
            {
                return info;
            }
            
            return default;
        }

        internal async Task<T> GetAssetAsync<T>(string path) where T : Asset
        {
            if (_guidPathDict.TryGetByValue(Paths.ClearPathSeparation(path), out var guid))
            {
                return await GetAssetAsync<T>(guid);
            }

            Debug.Error($"Asset doesn't exists at path: {path}");

            return default;
        }

        internal T GetAsset<T>(string path) where T : Asset
        {
            if (_guidPathDict.TryGetByValue(Paths.ClearPathSeparation(path), out var guid))
            {
                return GetAsset<T>(guid);
            }

            Debug.Error($"Asset doesn't exists at path: {path}");

            return default;
        }

        private async Task<T> GetAssetAsync<T>(Guid guid) where T : Asset
        {
            if (_databaseCache.GetAsset<T>(guid, out var asset))
            {
                return asset;
            }
            var assetContent = await Disk.GetAssetAsync(guid);

            if (assetContent.Success)
            {
                var assetMeta = Disk.GetAssetMeta(guid);

                asset = BuildAsset(assetContent.Info, assetMeta, guid, assetContent.RawData) as T;
                _databaseCache.PushAsset(guid, asset);
                return asset;
            }

            return null;
        }

        internal void UpdateReloadAsset(Guid guid)
        {
            if (_databaseCache.GetAsset(guid, out var asset))
            {
                var assetContent = Disk.GetAssetAsync(guid).GetAwaiter().GetResult();

                if (assetContent.Success)
                {
                    var assetMeta = Disk.GetAssetMeta(guid);

                    BuildAsset(assetContent.Info, assetMeta, guid, assetContent.RawData, true);
                }
            }
        }

        internal T GetAsset<T>(Guid guid) where T : Asset
        {
            if (guid == Guid.Empty)
                return null;

            if (_databaseCache.GetAsset<T>(guid, out var asset))
            {
                return asset;
            }

            var assetContent = Disk.GetAsset(guid);

            if (assetContent.Success)
            {
                var assetMeta = Disk.GetAssetMeta(guid);

                var rawAsset = BuildAsset(assetContent.Info, assetMeta, guid, assetContent.RawData);

                if (rawAsset == null)
                {
                    throw new Exception($"Fatal: Asset is null: {guid}");
                }

                asset = rawAsset as T;

                if (asset == null)
                {
                    Debug.Error($"Invalid asset cast to type: {typeof(T).Name}, asset type is: {rawAsset.GetType().Name}");
                }
                else
                {
                    asset.IsPhysicallyAvailable = true;
                    _databaseCache.PushAsset(guid, asset);
                }

                return asset;
            }
            else
            {
                Debug.Error($"Couldn't get asset: {guid}");
            }
            return null;
        }

        // TODO: cleanup this function.
        private Asset BuildAsset(AssetInfo info, AssetMeta meta, Guid guid, byte[] rawData, bool update = false)
        {
            var encoding = Encoding.Default;

            if (info.Type == AssetType.Text)
            {
                encoding = Encoding.UTF8;
            }

            using var mem = new MemoryStream(rawData);
            var reader = new BinaryReader(mem, encoding);

            if (_assetbuilder.TryGetValue(info.Type, out IAssetBuilder builder))
            {
                //if (info.IsEncrypted)
                //{
                //    await Task.Run(() => reader = new BinaryReader(AssetEncrypter.DecryptFromStream(reader.BaseStream, AssetUtils.ENCRYPTION_VERY_SECURE_PASSWORD)));
                //}

                //if (info.IsCompressed)
                //{
                //    await Task.Run(() => reader = new BinaryReader(AssetCompressor.DecompressStream(reader.BaseStream)));
                //}

                if (info.IsEncrypted)
                {
                    reader = new BinaryReader(AssetEncrypter.DecryptFromStream(reader.BaseStream, AssetUtils.ENCRYPTION_VERY_SECURE_PASSWORD));
                }

                if (info.IsCompressed)
                {
                    reader = new BinaryReader(AssetCompressor.DecompressStream(reader.BaseStream));
                }

                Asset asset = null;

                if (!update)
                {
                    asset = builder.BuildAsset(in info, meta, reader);
                }
                else if (_databaseCache.GetAsset(guid, out asset))
                {
                    builder.UpdateAsset(in info, asset, meta, reader);
                }

                reader.Dispose();
                return asset;
            }

            reader.Dispose();

            Debug.Error($"Builder for asset type '{info.Type}' was not found");
            return null;
        }

        internal void RemoveFromCache(Guid refId)
        {
            _databaseCache.Remove(refId);
        }
    }
}