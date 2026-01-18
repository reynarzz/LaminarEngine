using Engine;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Serialization
{
    /// <summary>
    /// Registers all the types so is faster to deserialize them.
    /// </summary>
    internal static class GfsTypeRegistry
    {
        private static readonly Dictionary<string, Type> _idToType = new();
        private static readonly Dictionary<Type, string> _typeToId = new();

        internal static void Register(Type type)
        {
            var id = ReflectionUtils.GetFullTypeName(type);
            if (!string.IsNullOrEmpty(id))
            {
                _idToType[id] = type;
                _typeToId[type] = id;
            }
        }
        internal static Type Resolve(string id)
        {
            if (!_idToType.TryGetValue(id, out var type))
            {
                Debug.Error($"Unregistered type id '{id}'");
            }

            return type;
        }
        internal static string GetId(Type type)
        {
            if (!_typeToId.TryGetValue(type, out var id))
            {
                Debug.Error($"Type '{type}' is not registered");
            }

            return id;
        }
        internal static bool IsRegistered(Type type)
        {
            return _typeToId.ContainsKey(type);
        }
        internal static void Clear()
        {
            _typeToId.Clear();
            _idToType.Clear();
        }
    }
}
