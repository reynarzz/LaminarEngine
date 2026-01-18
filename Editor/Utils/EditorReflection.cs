using Editor.Serialization;
using Engine;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utils
{
    internal class EditorReflection
    {
        internal static void RegisterTypeRecursive(Type type)
        {
            if (type == null)
                return;

            if (GfsTypeRegistry.IsRegistered(type))
            {
                return;
            }

            // Register first to break cycles
            GfsTypeRegistry.Register(type);

            if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
            {
                return;
            }

            // Array
            if (type.IsArray)
            {
                RegisterTypeRecursive(type.GetElementType());
                return;
            }

            // Generic
            if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                {
                    RegisterTypeRecursive(arg);
                }
            }

            // Base class
            RegisterTypeRecursive(type.BaseType);

            foreach (var memType in ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(type))
            {
                RegisterTypeRecursive(ReflectionUtils.GetMemberType(memType));
            }
        }
    }
}
