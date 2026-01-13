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

        internal static AssetResourceBase GetAssetFromGuid(Guid guid)
        {
            return IOLayer.GetDatabase().GetAsset<AssetResourceBase>(guid);
        }

        public static FontAsset GetFont(string path)
        {
            return Get<FontAsset>(path);
        }

        public static Texture2D GetTexture(string path)
        {
            return Get<Texture2D>(path);
        }

        public static AudioClip GetAudioClip(string path)
        {
            return Get<AudioClip>(path);
        }

        public static T Get<T>(string path) where T: AssetResourceBase
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
#endif
    }
}
