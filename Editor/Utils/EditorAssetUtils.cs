using Editor.Cooker;
using Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal static class EditorAssetUtils
    {
        private readonly static DefaultAssetMetaGenerator _defaultMetaGenerator = new();
        internal static AssetMeta GetAssetMeta(AssetResourceBase asset)
        {
            var path = EditorPaths.GetAbsolutePathSafe(asset.Path);
            var meta = GetMetaFromAbsolutePath(path, EditorIOLayer.Database.GetAssetInfo(asset.GetID()).Type);
            return meta;
        }

        internal static AssetMeta GetAssetMeta(string assetPathRelative, AssetType type)
        {
            var path = EditorPaths.GetAbsolutePathSafe(assetPathRelative);
            var meta = GetMetaFromAbsolutePath(path, type);
            return meta;
        }

        internal static void RefreshAssetDatabase()
        {
            EditorIOLayer.Instance.Refresh();
        }

        internal static void WriteMeta(string relativeAssetPath, AssetMeta meta)
        {
            File.WriteAllText(Paths.GetAbsoluteAssetPath(relativeAssetPath) + Paths.ASSET_META_EXT_NAME,
                JsonConvert.SerializeObject(meta, Formatting.Indented));
        }

        internal static AssetMeta GetMetaFromAbsolutePath(string absolutePath, AssetType assetType)
        {
            if (assetType == AssetType.Invalid)
            {
                Debug.Error("Asset type is invalid");
                return null;
            }

            var metaPath = absolutePath + Paths.ASSET_META_EXT_NAME;

            string metaJson = null;

            if (File.Exists(metaPath))
            {
                metaJson = File.ReadAllText(metaPath);
            }

            AssetMeta GetMeta<T>(string json) where T : AssetMeta, new()
            {
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }

                var metaOut = _defaultMetaGenerator.GetDefaultAssetMeta(absolutePath, assetType);

                if (metaOut != null)
                {
                    return metaOut;
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
                AssetType.Tilemap => GetMeta<TilemapMeta>(metaJson),
                _ => GetMeta<DefaultMetaFile>(metaJson)
            };
        }

        public static bool GetMetaGuid(string path, out Guid guid)
        {
            guid = default;
            string metaJson = null;

            if (!path.EndsWith(Paths.ASSET_META_EXT_NAME))
            {
                path = path + Paths.ASSET_META_EXT_NAME;
            }
            if (File.Exists(path))
            {
                metaJson = File.ReadAllText(path);

                guid = JsonConvert.DeserializeObject<DefaultMetaFile>(metaJson).GUID;
                return true;
            }

            return false;
        }

        internal static Type AssetTypeToType(this AssetType type)
        {
            Type clrType = null;

            switch (type)
            {
                case AssetType.Texture:
                    clrType = typeof(Texture);
                    break;
                case AssetType.Audio:
                    clrType = typeof(AudioClip);
                    break;
                case AssetType.Text:
                    clrType = typeof(TextAsset);
                    break;
                case AssetType.Shader:
                    clrType = typeof(Shader);
                    break;
                case AssetType.Font:
                    clrType = typeof(FontAsset);
                    break;
                case AssetType.AnimationClip:
                    clrType = typeof(AnimationClip);
                    break;
                case AssetType.AnimationController:
                    clrType = typeof(AnimatorController);
                    break;
                case AssetType.Material:
                    clrType = typeof(Material);
                    break;
                case AssetType.ShaderV2:
                    clrType = typeof(Shader);
                    break;
                case AssetType.Scene:
                    clrType = typeof(SceneAsset);
                    break;
                case AssetType.Tilemap:
                    clrType = typeof(TilemapAsset);
                    break;
                case AssetType.Script:
                    //clrType = typeof(ScriptAsset);
                    break;
                case AssetType.RenderTexture:
                    clrType = typeof(RenderTexture);
                    break;
                default:
                    break;
            }

            return clrType;
        }
    }
}
