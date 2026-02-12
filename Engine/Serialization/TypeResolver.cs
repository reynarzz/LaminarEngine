using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    internal interface ITypeResolver
    {
        static abstract bool ResolveType(ClassData prop, out Type type);
        static abstract bool ResolveType(ComponentIR prop, out Type type);
        static abstract bool ResolveType(SerializedPropertyIR prop, out Type type);
        static abstract bool ResolveType(EnumIRValue prop, out Type type);
    }
    // TODO: These types should implement a interface that exposes a 'string InternalType' property, so I only would need to use one function. 
    internal struct TypeResolver : ITypeResolver
    {
        public static bool ResolveType(ComponentIR prop, out Type type)
        {
#if SHIP_BUILD
            return Generated.TypeRegistryRuntime.ResolveType(prop.TypeId, out type);
#else
            return ReflectionUtils.ResolveType(prop.InternalType, out type);
#endif
        }

        public static bool ResolveType(ClassData prop, out Type type)
        {
#if SHIP_BUILD
            return Generated.TypeRegistryRuntime.ResolveType(prop.TypeId, out type);
#else
            return ReflectionUtils.ResolveType(prop.InternalType, out type);
#endif
        }

        public static bool ResolveType(SerializedPropertyIR prop, out Type type)
        {
#if SHIP_BUILD
            return Generated.TypeRegistryRuntime.ResolveType(prop.TypeId, out type);
#else
            return ReflectionUtils.ResolveType(prop.InternalType, out type);
#endif
        }

        public static bool ResolveType(EnumIRValue prop, out Type type)
        {
#if SHIP_BUILD
            return Generated.TypeRegistryRuntime.ResolveType(prop.TypeId, out type);
#else
            return ReflectionUtils.ResolveType(prop.EnumInternalType, out type);
#endif
        }
    }
}