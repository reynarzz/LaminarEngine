using Engine;
using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Drawers
{
    public class SerializedProperty
    {
        private object _target;
        private readonly MemberInfo _property;
        public string Name => _property.Name;
        public Type Type { get; }
        public SerializedProperty( MemberInfo member)
        {
            Type = ReflectionUtils.GetMemberType(member);
            _property = member;
        }
        internal void SetTarget(object target)
        {
            _target = target;
        }
        public object GetValue()
        {
            return ReflectionUtils.GetMemberValue(_target, _property);
        }

        public void SetValue(object value)
        {
            ReflectionUtils.SetMemberValue(_target, _property, value);
        }
    }

    public abstract class ComponentDrawer
    {
        private Dictionary<string, SerializedProperty> _properties = new();
        internal void InitializeProperties(Type target)
        {
            _properties.Clear();
            var members = ReflectionUtils.GetAllMembersWithAttribute<SerializedFieldAttribute>(target, true);
            foreach (var member in members)
            {
                _properties.Add(member.Name, new SerializedProperty(member));
            }
        }

        internal virtual protected void Open(object target)
        {
            foreach (var properties in _properties.Values)
            {
                properties.SetTarget(target);
            }
        }

        protected SerializedProperty GetProperty(string name)
        {
            if(_properties.TryGetValue(name, out var property)) 
                return property;

            Debug.Error($"Property named: '{name}' doesn't exist or is not part of the current target.");
            return null;
        }

        internal virtual protected void Close()
        {
            _properties.Clear();
        }
        internal protected abstract void Draw(object target, Action defaultInspectorDrawer);
    }

    public abstract class ComponentDrawer<T> : ComponentDrawer where T : Component
    {
        internal sealed override protected void Open(object target)
        {
            OnOpen();
        }
        internal virtual protected void OnOpen() { }
        internal virtual protected void OnClose() { }
        protected sealed internal override void Draw(object target, Action defaultDrawer)
        {
            OnDrawInspector(target as T, defaultDrawer);
        }

        internal virtual protected void OnDrawInspector(T target, Action defaultDrawer)
        {
            defaultDrawer?.Invoke();
        }
    }
}
