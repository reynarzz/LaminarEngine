using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    public abstract class PropertyDrawer
    {
        internal abstract protected bool DrawProperty(Type type, string name, object target, in object valueIn,
                                                      out object valueOut, Func<bool> defaultPropertyDrawer);
    }

    public abstract class PropertyDrawer<T, V> : PropertyDrawer
    {
        protected sealed internal override bool DrawProperty(Type type, string name, object target, in object valueIn,
                                                      out object valueOut, Func<bool> defaultPropertyDrawer)
        {
            var valueInV = valueIn != null ? (V)valueIn : default;
            return OnDrawProperty(type, name, target, in valueInV, out valueOut, defaultPropertyDrawer);
        }

        internal virtual protected bool OnDrawProperty(Type type, string name, object target, in V valueIn,
                                                      out object valueOut, Func<bool> defaultPropertyDrawer)
        {
            valueOut = null;
            return defaultPropertyDrawer?.Invoke() ?? false;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PropertyDrawerAttribute : Attribute
    {
        internal string PropertyName { get; }
        private PropertyDrawerAttribute()
        {
        }

        public PropertyDrawerAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
