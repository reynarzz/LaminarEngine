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

        public static bool IsDefaultRef(this SerializedType type)
        {
            switch (type)
            {
                case SerializedType.Component:
                case SerializedType.Actor:
                case SerializedType.TextureAsset:
                case SerializedType.MaterialAsset:
                case SerializedType.ShaderAsset:
                case SerializedType.AudioClipAsset:
                case SerializedType.AnimationAsset:
                case SerializedType.AnimatorControllerAsset:
                case SerializedType.RenderTextureAsset:
                case SerializedType.ScriptableObject:
                case SerializedType.Tilemap:
                case SerializedType.Prefab:
                case SerializedType.Scene:
                    return true;
            }

            if(type.IsEObject())
            {
                Debug.Warn($"EObject type '{type}' is not a default reference");
            }
            return false;
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
