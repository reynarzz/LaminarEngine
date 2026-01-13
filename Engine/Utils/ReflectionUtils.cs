using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Engine.Utils
{
    internal class ReflectionUtils
    {
        private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public
                                         | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        public static object GetDefaultValueInstance(Type type, int length = 0)
        {
            if (type.IsValueType)
            {
                if (type.IsEnum)
                {
                    Array values = Enum.GetValues(type);
                    return values.Length > 0 ? values.GetValue(0)! : Activator.CreateInstance(type);
                }
                else
                {
                   return Activator.CreateInstance(type);
                }
            }
            else
            {
                if (type == typeof(string))
                {
                    return string.Empty;
                }
                if (type.IsArray)
                {
                    return Array.CreateInstance(type.GetElementType(), length);
                }
                else if (type.IsClass && !type.IsAssignableTo(typeof(IObject)))
                {
                    try
                    {
                        return Activator.CreateInstance(type);
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                return null;
            }
        }
        public static Type NormalizeType(object obj)
        {
            var type = obj.GetType();
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }
        public static IEnumerable<PropertyInfo> GetAllPropertiesWithAttribute<T>(Type type, bool inherit = true, BindingFlags flags = _flags) where T : Attribute
        {
            while (type != null && type != typeof(object))
            {
                foreach (var prop in type.GetProperties(flags))
                {
                    if (prop.IsDefined(typeof(T), inherit) && prop.SetMethod != null)
                        yield return prop;
                }
                type = type.BaseType;
            }
        }

        public static IEnumerable<MemberInfo> GetAllMembersWithAttribute<T>(Type type, bool inherit = true, bool order = false,
                                                                            BindingFlags flags = _flags) where T : Attribute
        {
            while (type != null && type != typeof(object))
            {
                var members = type.GetMembers(flags)
                                  .Where(m => (m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property) &&
                                         m.IsDefined(typeof(T), inherit));

                if (order)
                {
                    members = members.OrderBy(m => m.MetadataToken);
                }

                foreach (var member in members)
                    yield return member;

                type = type.BaseType;
            }
        }

        public static object EnsureCount(object collection, int targetCount)
        {
            if (collection is Array array)
            {
                var elementType = array.GetType().GetElementType()!;
                int oldCount = array.Length;

                if (oldCount >= targetCount)
                    return array;

                var newArray = Array.CreateInstance(elementType, targetCount);
                Array.Copy(array, newArray, oldCount);
                return newArray;
            }

            if (collection is IList list)
            {
                var type = list.GetType();
                var elementType = type.IsGenericType ? type.GetGenericArguments()[0] : throw new InvalidOperationException("Non-generic IList not supported");

                object empty = elementType.IsValueType ? Activator.CreateInstance(elementType)! : null;

                while (list.Count < targetCount)
                {
                    list.Add(empty);
                }

                return list;
            }

            return collection;
        }

        public static IEnumerable<MemberInfo> GetAllMembersWithAttributes(Type type, Type[] attributeTypes, bool inherit = true,
                                                                          bool order = false, BindingFlags flags = _flags)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (attributeTypes == null || attributeTypes.Length == 0)
                throw new ArgumentException("At least one attribute type must be provided.", nameof(attributeTypes));

            foreach (var attrType in attributeTypes)
            {
                if (!typeof(Attribute).IsAssignableFrom(attrType))
                    throw new ArgumentException($"{attrType} is not an Attribute type.");
            }

            while (type != null && type != typeof(object))
            {
                var members = type.GetMembers(flags)
                                  .Where(m => (m.MemberType == MemberTypes.Field ||
                                              m.MemberType == MemberTypes.Property) &&
                                              attributeTypes.Any(a => m.IsDefined(a, inherit)));

                if (order)
                {
                    members = members.OrderBy(m => m.MetadataToken);
                }

                foreach (var member in members)
                {
                    yield return member;
                }

                type = type.BaseType;
            }
        }


        public static void SetMemberValue(object target, object value, string memberName, BindingFlags flags = _flags)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var type = target.GetType();

            while (type != null)
            {
                var prop = type.GetProperty(memberName, flags);
                if (prop != null)
                {
                    SetMemberValue(target, prop, value);
                    return;
                }

                var field = type.GetField(memberName, flags);
                if (field != null)
                {
                    SetMemberValue(target, field, value);
                    return;
                }

                type = type.BaseType;
            }

            Debug.Error($"Could not find member named: {memberName}");
        }

        public static MemberInfo GetMember(Type type, string name, BindingFlags flags = _flags)
        {
            while (type != null)
            {
                var prop = type.GetProperty(name, flags);
                if (prop != null)
                {
                    return prop;
                }

                var field = type.GetField(name, flags);
                if (field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static void SetMemberValue(object target, MemberInfo member, object value)
        {
            switch (member)
            {
                case PropertyInfo prop:
                    {
                        var setter = prop.SetMethod ?? prop.GetSetMethod(true);
                        if (setter == null)
                        {
                            Debug.Warn($"Set method for property: '{prop.Name}' was not found.");
                            return;
                        }

                        setter.Invoke(target, BindingFlags.InvokeMethod, Type.DefaultBinder, new object[] { value }, CultureInfo.InvariantCulture);
                        break;
                    }

                case FieldInfo field:
                    {
                        var flags = BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic;

                        field.SetValue(target, value, flags, Type.DefaultBinder, CultureInfo.InvariantCulture);
                        break;
                    }

                default:
                    throw new NotSupportedException("Unsupported member type: " + member.GetType());
            }
        }

        public static void SetMemberValueSafe<T>(object target, T value, string propertyName, int index, Func<T, object> valueConverter = null)
        {
            MemberInfo member = null;
            if (target is IList listV)
            {
                if (listV.Count > index)
                {
                    member = GetMember(listV[index]?.GetType(), propertyName);
                }
            }
            else
            {
                member = GetMember(target?.GetType(), propertyName);
            }

            if (target is IList list)
            {
                if (member == null)
                {
                    if (list.Count > index)
                    {
                        list[index] = value;
                    }
                }
                else
                {
                    var obj = list[index];
                    SetMemberValue(obj, member, value);
                    list[index] = obj;
                }
            }
            else
            {
                if (member == null)
                {
                    Debug.Error("Property is null, can't set member value.");
                    return;
                }

                if (valueConverter == null)
                {
                    SetMemberValue(target, member, value);
                }
                else
                {
                    SetMemberValue(target, member, valueConverter.Invoke(value));
                }
            }
        }

        public static void SetMemberValueSafe<T>(object target, T value, MemberInfo prop, int index, Func<T, object> valueConverter = null)
        {
            if (target is IList list)
            {
                list[index] = value;
            }
            else
            {
                if (prop == null)
                {
                    Debug.Error("Property is null, can't set member value.");
                    return;
                }

                if (valueConverter == null)
                {
                    SetMemberValue(target, prop, value);
                }
                else
                {
                    SetMemberValue(target, prop, valueConverter.Invoke(value));
                }
            }
        }

        public static bool ResolveType(string name, out Type type)
        {
            type = Type.GetType(name);
            if (type != null)
            {
                return true;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(name, throwOnError: false);
                if (type != null)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetFullTypeName(Type type)
        {
            if (type == null)
                return string.Empty;

            return StripAssemblyMetadata(type.AssemblyQualifiedName);
        }

        public static string StripAssemblyMetadata(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return typeName;

            return Regex.Replace(typeName, @",\s*Version=[^,\]]+|\s*,\s*Culture=[^,\]]+|\s*,\s*PublicKeyToken=[^,\]]+", string.Empty);
        }

        public static object GetMemberValue(object obj, MemberInfo member)
        {
            if (obj == null)
                return null;

            switch (member)
            {
                case PropertyInfo prop:
                    {
                        var getter = prop.GetMethod ?? prop.GetGetMethod(true);
                        if (getter == null)
                            throw new InvalidOperationException($"{prop.Name} has no getter.");

                        return getter.Invoke(obj, BindingFlags.InvokeMethod, Type.DefaultBinder, Array.Empty<object>(), CultureInfo.InvariantCulture);
                    }

                case FieldInfo field:
                    {
                        return field.GetValue(obj);
                    }

                default:
                    throw new NotSupportedException("Unsupported member type: " + member.GetType());
            }
        }

        public static bool IsUserDefinedStruct(MemberInfo member)
        {
            var type = GetMemberType(member);

            return IsUserDefinedStruct(type);
        }


        public static bool IsUserDefinedStruct(Type type)
        {
            if (type != null)
            {
                return type.IsValueType && !type.IsPrimitive && !type.IsEnum &&
                    !type.Namespace.Equals(typeof(vec2).Namespace) && type != typeof(Color) && type != typeof(Color32);
            }
            return false;
        }

        public static bool IsInternalType(MemberInfo member)
        {
            var type = GetMemberType(member);
            return IsInternalType(type);
        }

        public static bool IsInternalType(Type type)
        {
            if (type != null)
            {
                return type.IsValueType && (type.IsPrimitive || type.IsEnum ||
                       type.Namespace.Equals(typeof(vec2).Namespace) ||
                       type == typeof(Color) || type == typeof(Color32)) ||
                       type == typeof(string);
            }
            return false;
        }

        public static bool IsCollectionOfInternalTypes(Type type)
        {
            if (!IsCollection(type, out var collectionType))
                return false;

            var types = GetCollectionElementsType(type);

            if (collectionType == CollectionType.Dictionary)
            {
                return IsInternalType(types[0]) && IsInternalType(types[1]);
            }

            return IsInternalType(types[0]);
        }

        public static Type[] GetCollectionElementsType(Type type)
        {
            return GetCollectionElementsType(type, out _);
        }

        public static Type[] GetCollectionElementsType(Type type, out CollectionType collectionType)
        {
            if (!IsCollection(type, out collectionType))
                return null;

            switch (collectionType)
            {
                case CollectionType.Array:
                    return [type.GetElementType()];
                case CollectionType.List:
                case CollectionType.Stack:
                case CollectionType.Queue:
                case CollectionType.Hashset:
                case CollectionType.Dictionary:
                    return type.GetGenericArguments();
            }

            return null;
        }


        public static Type GetMemberType(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo p => p.PropertyType,
                FieldInfo f => f.FieldType,
                _ => null
            };
        }

        public static Type GetMemberType(Type type, string name, BindingFlags flags = _flags)
        {
            while (type != null)
            {
                var prop = type.GetProperty(name, flags);
                if (prop != null)
                {
                    return prop.PropertyType;
                }

                var field = type.GetField(name, flags);
                if (field != null)
                {
                    return field.FieldType;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static int GetPropertiesCount(MemberInfo member, BindingFlags flags = _flags, params Attribute[] attributes)
        {
            switch (member)
            {
                case PropertyInfo prop:
                    {
                        return prop?.PropertyType?.GetProperties(flags)?.Length ?? 0;
                    }
                case FieldInfo field:
                    {
                        return field?.FieldType?.GetFields(flags)?.Length ?? 0;
                    }
                default:
                    return 0;
            }
        }

        public static bool IsCollection(MemberInfo member)
        {
            var type = GetMemberType(member);
            return IsCollection(type);
        }

        internal enum CollectionType
        {
            None,
            Array,
            List,
            Dictionary,
            Stack,
            Queue,
            Hashset
        }

        public static bool IsCollection(Type type)
        {
            return IsCollection(type, out _);
        }

        public static bool IsCollection(Type type, out CollectionType collectionType)
        {
            collectionType = CollectionType.None;

            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    collectionType = CollectionType.List;
                    return true;
                }
                else if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    collectionType = CollectionType.Dictionary;
                    return true;
                }
                else if (type.GetGenericTypeDefinition() == typeof(Stack<>))
                {
                    collectionType = CollectionType.Stack;
                    return true;
                }
                else if (type.GetGenericTypeDefinition() == typeof(Queue<>))
                {
                    collectionType = CollectionType.Queue;
                    return true;
                }
                else if (type.GetGenericTypeDefinition() == typeof(HashSet<>))
                {
                    collectionType = CollectionType.Hashset;
                    return true;
                }
            }
            else
            {
                collectionType = CollectionType.Array;
                return type.IsArray;
            }
            return false;
        }

        public static bool IsEObject(Type t)
        {
            return typeof(EObject).IsAssignableFrom(t);// || typeof(IObject).IsAssignableFrom(t);
        }

        internal static bool HasAnySerializedMemberWithType(Type target, Type searchedType)
        {
            var visited = new HashSet<Type>();
            return ContainsType(target, searchedType, visited);
        }

        private static bool ContainsType(Type current, Type searched, HashSet<Type> visited)
        {
            if (current == null)
                return false;

            // Prevent infinite recursion on cyclic graphs
            if (!visited.Add(current))
                return false;

            // Exact or assignable match
            if (current == searched || searched.IsAssignableFrom(current))
                return true;

            // Array
            if (current.IsArray)
                return ContainsType(current.GetElementType(), searched, visited);

            // Nullabl
            if (Nullable.GetUnderlyingType(current) is Type nullable)
                return ContainsType(nullable, searched, visited);

            // Generic arguments
            if (current.IsGenericType)
            {
                foreach (var arg in current.GetGenericArguments())
                {
                    if (ContainsType(arg, searched, visited))
                        return true;
                }
            }

            // Stop on internal types
            if (IsInternalType(current))
                return false;

            foreach (var member in GetAllMembersWithAttribute<SerializedFieldAttribute>(current))
            {
                var memberType = GetMemberType(member);
                if (ContainsType(memberType, searched, visited))
                    return true;
            }

            return false;
        }
    }
}