#if !SHIP_BUILD
using Engine.Serialization;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Serialization
{
    // TODO: These types should implement a interface that exposes a 'string InternalType' property, so I only would need to use one function. 
    internal struct CombinedTypeResolver : ITypeResolver
    {
        public static bool ResolveType(ComponentIR prop, out Type type)
        {
            return Resolve(prop.InternalType, prop.TypeId, out type);
        }

        public static bool ResolveType(ClassData prop, out Type type)
        {
            return Resolve(prop.InternalType, prop.TypeId, out type);
        }

        public static bool ResolveType(SerializedPropertyIR prop, out Type type)
        {
            return Resolve(prop.InternalType, prop.TypeId, out type);
        }

        public static bool ResolveType(EnumIRValue prop, out Type type)
        {
            return Resolve(prop.EnumInternalType, prop.TypeId, out type);
        }

        private static bool Resolve(string internalType, Guid id, out Type type)
        {
            type = null;
            var success = false;//Generated.TypeRegistryRuntime.ResolveType(id, out type);

            if (!success)
            {
                success = ReflectionUtils.ResolveType(internalType, out type);

                if (success)
                {
                    // Debug.Warn($"id: {id} did not worked for type: {type.FullName}");

                }
            }
            else
            {
                // Debug.Log($"id: {id} worked for type: {type.FullName}");
            }
            return success;
        }
    }
}
#endif