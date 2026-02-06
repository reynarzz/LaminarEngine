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
        static abstract bool ResolveType(ComplexTypeData prop, out Type type);
        static abstract bool ResolveType(ComponentIR prop, out Type type);
        static abstract bool ResolveType(SerializedPropertyIR prop, out Type type);
    }
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

        static bool ITypeResolver.ResolveType(ComplexTypeData prop, out Type type)
        {
#if SHIP_BUILD
            return Generated.TypeRegistryRuntime.ResolveType(prop.TypeId, out type);
#else
            return ReflectionUtils.ResolveType(prop.InternalType, out type);
#endif
        }

        static bool ITypeResolver.ResolveType(SerializedPropertyIR prop, out Type type)
        {
#if SHIP_BUILD
            return Generated.TypeRegistryRuntime.ResolveType(prop.TypeId, out type);
#else
            return ReflectionUtils.ResolveType(prop.InternalType, out type);
#endif
        }
    }
}