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
        public static bool IsEObject(this SerializedType type)
        {
            return (type & SerializedType.EObjectFlag) != 0;
        }
        public static bool IsCollection(this SerializedType type)
        {
            return (type & SerializedType.CollectionFlag) != 0;
        }

        public static bool IsComplexClass(this SerializedType type)
        {
            return (type & SerializedType.ComplexClass) != 0;
        }
        
        public static ulong GetIdentity(this SerializedType type)
        {
            // Strip the flags (bits 0-19)
            ulong identity = (ulong)type & ~TraitMask;
            // Shift back down so we get "1007" instead of "1055834112"
            return identity >> 20;
        }
    }
}
