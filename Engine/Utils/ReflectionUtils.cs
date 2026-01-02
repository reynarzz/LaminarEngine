using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    internal class ReflectionUtils
    {
        private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public
                                         | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
        IEnumerable<T> GetAllAttributes<T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
                                            Type type) where T : Attribute
        {
            var result = new List<T>();

            while (type != null && type != typeof(object))
            {
                result.AddRange(type.GetCustomAttributes(typeof(T), inherit: false).Cast<T>());
                type = type.BaseType;
            }

            return result;
        }

        public static Type NormalizeType(object obj)
        {
            var type = obj.GetType();
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }
        public static IEnumerable<PropertyInfo> GetAllPropertiesWithAttribute<T>(Type type, bool inherit = true) where T : Attribute
        {
            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly;

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
                var members = type
                    .GetMembers(flags)
                    .Where(m =>
                        (m.MemberType == MemberTypes.Field ||
                         m.MemberType == MemberTypes.Property) &&
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
        public static object GetMemberValue(object obj, MemberInfo member)
        {
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
                    !type.Namespace.Equals(typeof(vec2).Namespace);
            }
            return false;
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

        public static bool IsCollection(Type type)
        {
            return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) ||
                                           type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                 || type.IsArray;
        }

        public static bool IsEObject(Type t)
        {
            return typeof(EObject).IsAssignableFrom(t);// || typeof(IObject).IsAssignableFrom(t);
        }
    }
}