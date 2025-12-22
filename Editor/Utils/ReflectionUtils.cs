using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Editor
{
    public static class ReflectionUtil
    {
        public static object GetValue(object obj, PropertyInfo prop)
            => prop.GetValue(obj);

        public static void SetValue(object obj, PropertyInfo prop, object value)
            => prop.SetValue(obj, value);

        public static bool IsEnumerable(Type t)
            => typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string);

        public static bool IsEObject(Type t)
            => typeof(EObject).IsAssignableFrom(t);

        public static bool IsComponent(Type t)
            => typeof(Component).IsAssignableFrom(t);

        public static bool IsEnum(Type t)
            => t.IsEnum;

        public static Type GetEnumerableElementType(Type t)
            => t.IsArray ? t.GetElementType() : t.GetGenericArguments()[0];
    }

}
