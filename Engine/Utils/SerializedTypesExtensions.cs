using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    internal static class SerializedTypeExtensions
    {
        private const ulong TraitMask = 0xFFFFFUL;

        public static bool IsSimple(this SerializedType type) 
        {
            return (type & SerializedType.SimpleFlag) != 0; 
        }
        public static bool IsAsset(this SerializedType type) 
        {
            return (type & SerializedType.AssetFlag) != 0; 
        }

        public static bool IsDefaultAssetRef(this SerializedType type)
        {
            return (type & (SerializedType.Component |
                            SerializedType.Actor |
                            SerializedType.TextureAsset |
                            SerializedType.MaterialAsset |
                            SerializedType.ShaderAsset |
                            SerializedType.AudioClipAsset |
                            SerializedType.AnimationAsset |
                            SerializedType.AnimatorControllerAsset |
                            SerializedType.RenderTextureAsset |
                            SerializedType.ScriptableObject)) != 0;
        }

        public static bool IsEObject(this SerializedType type)
        {
            return (type & SerializedType.EObjectFlag) != 0;
        }
        public static bool IsCollection(this SerializedType type)
        {
            return (type & SerializedType.CollectionFlag) != 0;
        }

        public static bool IsClass(this SerializedType type)
        {
            return (type & SerializedType.ClassFlag) != 0;
        }
        
        public static ulong GetIdentity(this SerializedType type)
        {
            // Strip the flags: bits 0 to 19, and then, shift back down.
            return ((ulong)type & ~TraitMask) >> 20;
        }
    }
}
