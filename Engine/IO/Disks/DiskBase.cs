using Newtonsoft.Json;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.IO
{
    internal abstract class DiskBase
    {
        public AssetsDatabaseInfo AssetDatabaseInfo { get; protected set; } = new();


        public abstract bool Initialize();

        public struct RawAssetContent
        {
            public AssetInfo Info { get; set; }
            public byte[] RawData { get; set; }
            public bool Success => RawData != null;
        }

        public async Task<RawAssetContent> GetAssetAsync(Guid guid)
        {
            if (AssetDatabaseInfo.Assets.TryGetValue(guid, out var info))
            {
                var data = await LoadAssetFromDiskAsync(guid);

                if (data == null)
                {
                    Debug.Error("Fatal: Can't load asset from disk, is in database table but contents are not in disk?");
                    return default;
                }

                return new RawAssetContent() { Info = info, RawData = data };
            }

            return default;
        }

        internal KeyValuePair<Guid, AssetInfo>[] GetAssetsInfo(AssetType assetType)
        {
            return AssetDatabaseInfo.Assets.Where(x => x.Value.Type == assetType).ToArray();
        }

        public AssetMeta GetAssetMeta(Guid guid)
        {
            if (AssetDatabaseInfo.Assets.TryGetValue(guid, out var info))
            {
                var bytes = LoadMetaFromDisk(guid);
                var json = Encoding.UTF8.GetString(bytes);

                switch (info.Type)
                {
                    case AssetType.Invalid:
                        break;
                    case AssetType.Texture:
                        return JsonConvert.DeserializeObject<TextureMetaFile>(json);
                    case AssetType.Audio:
                        return JsonConvert.DeserializeObject<AudioMetaFile>(json);
                    case AssetType.Text:
                        return JsonConvert.DeserializeObject<DefaultMetaFile>(json);
                    case AssetType.Shader:
                        return JsonConvert.DeserializeObject<DefaultMetaFile>(json);
                    case AssetType.ShaderV2:
                        return JsonConvert.DeserializeObject<DefaultMetaFile>(json);
                    case AssetType.Font:
                        return JsonConvert.DeserializeObject<DefaultMetaFile>(json);
                    case AssetType.AnimationClip:
                        return JsonConvert.DeserializeObject<DefaultMetaFile>(json);
                    case AssetType.AnimationController:
                        return JsonConvert.DeserializeObject<DefaultMetaFile>(json);
                    case AssetType.Material:
                        return JsonConvert.DeserializeObject<DefaultMetaFile>(json);
                    case AssetType.Tilemap:
                        return JsonConvert.DeserializeObject<TilemapMeta>(json);
                    default:
                        throw new NotImplementedException($"Asset type for meta is not implemented: {info.Type}");
                }
            }

            return null;
        }

        protected abstract Task<byte[]> LoadAssetFromDiskAsync(Guid guid);
        protected abstract byte[] LoadAssetFromDisk(Guid guid);
        protected abstract byte[] LoadMetaFromDisk(Guid guid);
    }
}
