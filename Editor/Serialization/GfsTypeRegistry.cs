using Engine;
using Engine.Layers;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        internal static Type GameAppType { get; private set; }
        internal static List<Type> GameAppComponentTypes { get; private set; } = new();

        internal static Assembly EngineAssembly { get; }
        internal static Assembly SharedTypesAssembly { get; }
        internal static Assembly EditorAssembly { get; }
        internal static Assembly GameAssembly { get; set; }

        static GfsTypeRegistry()
        {
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (item.GetName().Name.Equals("Editor"))
                {
                    EditorAssembly = item;
                }
                else if (item.GetName().Name.Equals("Engine"))
                {
                    EngineAssembly = item;
                }
                else if (item.GetName().Name.Equals("SharedTypes"))
                {
                    SharedTypesAssembly = item;
                }
            }
        }

        internal static void Register(Type type)
        {
            var id = ReflectionUtils.GetFullTypeName(type);
            if (!string.IsNullOrEmpty(id))
            {
                if (!_idToType.ContainsKey(id))
                {
                    _idToType[id] = type;
                }

                if (!_typeToId.ContainsKey(type))
                {
                    _typeToId[type] = id;

                    if (!type.IsAbstract && type.IsAssignableTo(typeof(Component)) &&
                       IsTypeFromGameAssembly(type))
                    {
                        GameAppComponentTypes.Add(type);
                    }
                }

                if (!type.IsAbstract && type.IsAssignableTo(typeof(ApplicationLayer)) &&
                    IsTypeFromGameAssembly(type))
                {
                    GameAppType = type;
                }
            }
        }

        private static bool IsTypeFromGameAssembly(Type type)
        {
            //return type.Assembly.GetName().Name.Equals(EditorPaths.GAME_PROJECT_NAME);
            return type.Assembly == GameAssembly;
        }

        internal static void RegisterRecursive(Type type)
        {
            if (type == null)
                return;

            if (IsRegistered(type))
            {
                return;
            }

            // Register first to break cycles
            Register(type);

            if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
            {
                return;
            }

            if (type.IsArray)
            {
                RegisterRecursive(type.GetElementType());
                return;
            }

            if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                {
                    RegisterRecursive(arg);
                }
            }

            RegisterRecursive(type.BaseType);

            foreach (var memType in ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(type))
            {
                RegisterRecursive(ReflectionUtils.GetMemberType(memType));
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
            GameAppComponentTypes.Clear();
            GameAppType = null;
            GameAssembly = null;
        }
    }
}
