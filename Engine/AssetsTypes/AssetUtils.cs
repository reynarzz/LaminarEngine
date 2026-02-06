using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal static class AssetUtils
    {
        public const string ENCRYPTION_VERY_SECURE_PASSWORD = "ThisDefinitelyShouldNotBeHere";
        public static class GFSFileFormat
        {
            public const string HEADER = "GFSD";
        }

        internal static void WriteMeta(string relativeAssetPath, AssetMetaFileBase meta)
        {
            File.WriteAllText(Paths.GetAbsoluteAssetPath(relativeAssetPath) + Paths.ASSET_META_EXT_NAME,
                JsonConvert.SerializeObject(meta, Formatting.Indented));
        }


        internal static AssetMetaFileBase GetMeta(string path, AssetType assetType)
        {
            string metaJson = null;

            if (File.Exists(path))
            {
                metaJson = File.ReadAllText(path);
            }

            AssetMetaFileBase GetMeta<T>(string json) where T : AssetMetaFileBase, new()
            {
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }

                return new T { GUID = Guid.NewGuid() };
            }

            return assetType switch
            {
                AssetType.Invalid => GetMeta<DefaultMetaFile>(metaJson),
                AssetType.Texture => GetMeta<TextureMetaFile>(metaJson),
                AssetType.Audio => GetMeta<AudioMetaFile>(metaJson),
                AssetType.Text => GetMeta<DefaultMetaFile>(metaJson),
                AssetType.Shader => GetMeta<DefaultMetaFile>(metaJson),
                _ => GetMeta<DefaultMetaFile>(metaJson)
            };
        }

        public static bool GetMetaGuid(string path, out Guid guid)
        {
            guid = default;
            string metaJson = null;

            if (File.Exists(path))
            {
                metaJson = File.ReadAllText(path);

                guid = JsonConvert.DeserializeObject<DefaultMetaFile>(metaJson).GUID;
                return true;
            }

            return false;
        }
    }
}
