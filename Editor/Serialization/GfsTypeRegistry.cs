using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    public static class GfsTypeRegistry
    {
        private static readonly Dictionary<string, Type> IdToType = new();
        private static readonly Dictionary<Type, string> TypeToId = new();

        public static void Register<T>(string id)
        {
            var type = typeof(T);
            IdToType[id] = type;
            TypeToId[type] = id;
        }

        public static Type Resolve(string id)
        {
            if (!IdToType.TryGetValue(id, out var type))
                throw new InvalidOperationException($"Unregistered type id '{id}'");

            return type;
        }

        public static string GetId(Type type)
        {
            if (!TypeToId.TryGetValue(type, out var id))
                throw new InvalidOperationException($"Type '{type}' is not registered");

            return id;
        }
    }
}
