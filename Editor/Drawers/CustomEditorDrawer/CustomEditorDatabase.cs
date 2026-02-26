using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    internal class CustomEditorDatabase
    {
        private static Dictionary<Type, Type> _targetToComponentDrawerTypes = new();
        private static Dictionary<Type, PropertyDrawerInfo> _targetToPropertyDrawerTypes = new();
        private static Dictionary<Type, ComponentDrawer> _componentDrawers = new();
        private static Dictionary<Type, PropertyDrawer> _propertyDrawers = new();

        private class PropertyDrawerInfo
        {
            public Type DrawerType { get; set; }
            public string MatchTargetPropertyName { get; set; }
            public Type MatchTargetPropertyType { get; set; }
        }

        internal static void InitCustomComponentDrawers(List<Type> customEditorTypes)
        {
            InitCustomDrawer(_targetToComponentDrawerTypes, customEditorTypes, typeof(ComponentDrawer<>));
        }

        internal static void InitCustomPropertiesDrawers(List<Type> customEditorTypes)
        {
            _targetToPropertyDrawerTypes.Clear();

            foreach (var type in customEditorTypes)
            {
                var targetType = GetDrawerTargetType(type, typeof(PropertyDrawer<,>));
                var attribute = type.GetCustomAttribute<PropertyDrawerAttribute>();

                _targetToPropertyDrawerTypes.Add(targetType, new PropertyDrawerInfo()
                {
                    DrawerType = type,
                    MatchTargetPropertyName = attribute?.PropertyName ?? string.Empty,
                    MatchTargetPropertyType = GetDrawerTargetType(type, typeof(PropertyDrawer<,>), 1)
                });
            }
        }

        private static void InitCustomDrawer(Dictionary<Type, Type> drawerTypes, List<Type> customDrawersTypes, Type baseDrawerType)
        {
            drawerTypes.Clear();

            foreach (var type in customDrawersTypes)
            {
                var targetType = GetDrawerTargetType(type, baseDrawerType);
                drawerTypes.Add(targetType, type);
            }
        }

        private static Type GetDrawerTargetType(Type type, Type drawerType, int argIndex = 0)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == drawerType)
                {
                    return type.GetGenericArguments()[argIndex];
                }

                type = type.BaseType;
            }

            return null;
        }
        internal static bool TryGetCustomPropertyDrawer(Type target, Type propertyType, string propertyName, out PropertyDrawer drawer)
        {
            drawer = null;

            if (target == null)
            {
                Debug.Error("Target is null, can't create custom editor.");
                return false;
            }
            if (!_targetToPropertyDrawerTypes.ContainsKey(target))
            {
                return false;
            }
            var drawerInfo = _targetToPropertyDrawerTypes[target];
            PropertyDrawer propDrawer = null;
            if (drawerInfo.MatchTargetPropertyType == propertyType &&
                    (drawerInfo.MatchTargetPropertyName.Equals(propertyName) || string.IsNullOrEmpty(drawerInfo.MatchTargetPropertyName)))
            {
                if (!_propertyDrawers.TryGetValue(target, out propDrawer))
                {
                    propDrawer = Activator.CreateInstance(drawerInfo.DrawerType) as PropertyDrawer;
                    _propertyDrawers.Add(target, propDrawer);
                }
            }
            drawer = propDrawer;

            return drawer != null;
        }

        internal static bool TryGetCustomComponentDrawer(Type target, out ComponentDrawer drawer)
        {
            drawer = null;

            if (target == null)
            {
                Debug.Error("Target is null, can't create custom editor.");
                return false;
            }
            if (!_targetToComponentDrawerTypes.ContainsKey(target))
            {
                return false;
            }
            if (!_componentDrawers.TryGetValue(target, out var editorDrawer))
            {
                editorDrawer = Activator.CreateInstance(_targetToComponentDrawerTypes[target]) as ComponentDrawer;
                editorDrawer.InitializeProperties(target);
                _componentDrawers.Add(target, editorDrawer);
            }

            drawer = editorDrawer;

            return true;
        }
    }
}
