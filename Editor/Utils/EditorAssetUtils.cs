using Editor.Cooker;
using Editor.Serialization;
using Editor.Utils;
using Engine;
using Engine.Layers;
using Engine.Utils;
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
        internal static AssetMeta GetAssetMeta(Asset asset)
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

        internal static Task RefreshAssetDatabase()
        {
            return EditorIOLayer.Instance.Refresh();
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
        internal static AssetType AssetToAssetType(this Type type)
        {
            if (type == null)
            {
                return AssetType.Invalid;
            }

            if (typeof(Texture).IsAssignableFrom(type)) 
                return AssetType.Texture;
            if (typeof(Texture2D).IsAssignableFrom(type))
                return AssetType.Texture2D;
            //if (typeof(Texture2D).IsAssignableFrom(type))
            //    return AssetType.Texture3D;
            if (typeof(Sprite).IsAssignableFrom(type))
                return AssetType.Sprite;
            if (typeof(AudioClip).IsAssignableFrom(type))
                return AssetType.Audio;
            if (typeof(TextAsset).IsAssignableFrom(type)) 
                return AssetType.Text;
            if (typeof(Shader).IsAssignableFrom(type)) 
                return AssetType.Shader;
            if (typeof(FontAsset).IsAssignableFrom(type))
                return AssetType.Font;
            if (typeof(AnimationClip).IsAssignableFrom(type))
                return AssetType.AnimationClip;
            if (typeof(AnimatorController).IsAssignableFrom(type)) 
                return AssetType.AnimatorController;
            if (typeof(Material).IsAssignableFrom(type))
                return AssetType.Material;
            if (typeof(SceneAsset).IsAssignableFrom(type)) 
                return AssetType.Scene;
            if (typeof(TilemapAsset).IsAssignableFrom(type)) 
                return AssetType.Tilemap;
            if (typeof(RenderTexture).IsAssignableFrom(type)) 
                return AssetType.RenderTexture;

            if (typeof(Shader).IsAssignableFrom(type)) 
                return AssetType.ShaderV2;

            return AssetType.Invalid;
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
                case AssetType.AnimatorController:
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
                case AssetType.Sprite:
                    clrType = typeof(Sprite);
                    break;
                case AssetType.Texture2D:
                    clrType = typeof(Texture2D);
                    break;
                case AssetType.Texture3D:
                    clrType = null;// typeof(Texture3D);
                    break;
                    // default:
                    // throw new NotImplementedException(type.ToString());
            }

            return clrType;
        }
        internal static void CreateScene(string relativeDir)
        {
            if (string.IsNullOrEmpty(relativeDir))
            {
                return;
            }
            var newScene = new Scene(Guid.NewGuid());
            var sceneIR = SceneSerializer.SerializeScene(newScene);
            var json = EditorJsonUtils.Serialize(sceneIR);

            var absPath = EditorPaths.GetAbsolutePathSafe(relativeDir);
            var dirName = Path.GetDirectoryName(absPath);
            if (Directory.Exists(dirName))
            {
                File.WriteAllText(absPath, json);
            }
            else
            {
                Debug.Error($"Directory doesn't exists: {dirName}, can't create asset");
            }

            RefreshAssetDatabase();
        }
        internal static string RemoveRootAssetFolder(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath) || relativePath.Equals(Paths.ASSETS_FOLDER_NAME, StringComparison.OrdinalIgnoreCase) ||
                relativePath.Equals(EditorPaths.CookerPaths.INTERNAL_ASSET_FOLDER_NAME, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            var rootFolder = string.Empty;
            if (relativePath.StartsWith(EditorPaths.CookerPaths.INTERNAL_ASSET_FOLDER_NAME))
            {
                rootFolder = EditorPaths.CookerPaths.INTERNAL_ASSET_FOLDER_NAME;
            }
            else if (relativePath.StartsWith(Paths.ASSETS_FOLDER_NAME))
            {
                rootFolder = Paths.ASSETS_FOLDER_NAME;
            }
            else
            {
                return relativePath;
            }

            return relativePath.Substring(rootFolder.Length + 1);
        }
        internal static void DeleAsset(string relativePath)
        {
            if (relativePath.Equals(Paths.ASSETS_FOLDER_NAME, StringComparison.OrdinalIgnoreCase) ||
                relativePath.Equals(EditorPaths.CookerPaths.INTERNAL_ASSET_FOLDER_NAME, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Error("Can't delete root asset folder: " + relativePath);
                return;
            }
            var path = EditorPaths.GetAbsolutePathSafe(relativePath);

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
                File.Delete(path + Paths.ASSET_META_EXT_NAME);
            }

            RefreshAssetDatabase();
        }

        internal static bool MoveAsset(string fromRelativePath, string toRelativePath, bool overwrite)
        {
            var currentFile = EditorPaths.GetAbsolutePathSafe(fromRelativePath);
            var newFilePath = EditorPaths.GetAbsolutePathSafe(toRelativePath);

            var exists = File.Exists(newFilePath);
            if (!exists || (exists && overwrite))
            {
                if (Directory.Exists(currentFile))
                {
                    Directory.Move(currentFile, newFilePath);

                    // TODO: folders will have a id assigned in the future, once that happens uncomment this.
                    // --Directory.Move(currentFile + Paths.ASSET_META_EXT_NAME, newFilePath + Paths.ASSET_META_EXT_NAME);
                }
                else
                {
                    File.Move(currentFile, newFilePath);
                    File.Move(currentFile + Paths.ASSET_META_EXT_NAME, newFilePath + Paths.ASSET_META_EXT_NAME);
                }

                // Update assets database to remove:
                RefreshAssetDatabase();

                return true;
            }

            return false;
        }

        internal static void CreateDirectory(string relativePathDir)
        {
            var absPath = EditorPaths.GetAbsolutePathSafe(relativePathDir);
            Directory.CreateDirectory(absPath);
            RefreshAssetDatabase();
        }
    }
}