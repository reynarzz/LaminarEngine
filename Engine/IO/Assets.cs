using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Engine.Layers;

namespace Engine
{
    public class Assets
    {
        // TODO:
        //public static async Task<TextAsset> GetTextAsync(string path)
        //{
        //    return await IOLayer.GetDatabase().GetAssetAsync<TextAsset>(path);
        //}
        // TODO:
        //public static async Task<Texture2D> GetTextureAsync(string path)
        //{
        //    return await IOLayer.GetDatabase().GetAssetAsync<Texture2D>(path);
        //}

        public static TextAsset GetText(string path)
        {
            return Get<TextAsset>(path);
        }

        /// <summary>
        /// Get the SpriteAtlas linked to a Texture2D.
        /// </summary>
        public static SpriteAtlas GetSpriteAtlas(string path)
        {
            var asset = Get<TextureAsset>(path);
            return asset?.Atlas; 
        }

        public static Shader GetShader(string path)
        {
            return Get<Shader>(path);
        }
        public static Material GetMaterial(string path)
        {
            return Get<Material>(path);
        }
        internal static Asset GetAssetFromGuid(Guid guid)
        {
            if(guid == Guid.Empty)
            {
                return null;
            }
            return IOLayer.GetDatabase().GetAsset<Asset>(guid);
        }

        public static FontAsset GetFont(string path)
        {
            return Get<FontAsset>(path);
        }

        public static Texture2D GetTexture(string path)
        {
            var asset = Get<TextureAsset>(path);
            return asset.Texture as Texture2D;
        }

        public static AudioClip GetAudioClip(string path)
        {
            return Get<AudioClip>(path);
        }

        public static TilemapAsset GetTilemap(string path)
        {
            return Get<TilemapAsset>(path);
        }
        public static Asset Get(string path)
        {
            return Get<Asset>(path);
        }

        internal static SceneAsset GetScene(string name)
        {
            return Get<SceneAsset>(name);
        }





        internal static T Get<T>(string path) where T : Asset
        {
#if DEBUG
            if (!string.IsNullOrEmpty(path) && !_loadedPaths.Contains(path))
                _loadedPaths.Add(path);
#endif

            return IOLayer.GetDatabase().GetAsset<T>(path);
        }

#if DEBUG
        private static List<string> _loadedPaths = new();
        public static string[] LoadedPaths()
        {
            return _loadedPaths.ToArray();
        }

        /// <summary>
        /// Destroy asset instance and removes it from cache.
        /// </summary>
        /// <param name="refId"></param>
        internal static void DestroyAsset(Guid refId)
        {
            if(refId == Guid.Empty)
            {
                return;
            }

            var asset = IOLayer.GetDatabase().GetAsset<Asset>(refId);

            if (asset)
            {
                asset.IsPhysicallyAvailable = false;
                IOLayer.GetDatabase().RemoveFromCache(refId);
            }
        }
#endif
    }
}
