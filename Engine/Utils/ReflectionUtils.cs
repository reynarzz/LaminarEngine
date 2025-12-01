using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    internal class ReflectionUtils
    {
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

        public static IEnumerable<MemberInfo> GetAllMembersWithAttribute<TAttr>(Type type, bool inherit = true) where TAttr : Attribute
        {
            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly;

            while (type != null && type != typeof(object))
            {
                foreach (var prop in type.GetProperties(flags))
                    if (prop.IsDefined(typeof(TAttr), inherit))
                        yield return prop;

                foreach (var field in type.GetFields(flags))
                    if (field.IsDefined(typeof(TAttr), inherit))
                        yield return field;

                type = type.BaseType;
            }
        }

        public static void SetMemberValue(object obj, MemberInfo member, object value)
        {
            switch (member)
            {
                case PropertyInfo prop:
                    {
                        var setter = prop.SetMethod ?? prop.GetSetMethod(true);
                        if (setter == null)
                            throw new InvalidOperationException($"{prop.Name} has no setter.");

                        setter.Invoke(
                            obj,
                            BindingFlags.InvokeMethod,
                            Type.DefaultBinder,
                            new object[] { value },
                            CultureInfo.InvariantCulture
                        );
                        break;
                    }

                case FieldInfo field:
                    {
                        field.SetValue(obj, value,
                            BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic,
                            Type.DefaultBinder,
                            CultureInfo.InvariantCulture);
                        break;
                    }

                default:
                    throw new NotSupportedException("Unsupported member type: " + member.GetType());
            }
        }

        public static Type GetMemberType(MemberInfo member)
        {
            return member switch
            {
                PropertyInfo p => p.PropertyType,
                FieldInfo f => f.FieldType,
                _ => throw new NotSupportedException($"Unsupported member: {member.MemberType}")
            };
        }
    }
}
