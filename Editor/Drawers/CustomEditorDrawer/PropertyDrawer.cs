using Editor.Utils;
using ImGuiNET;
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

        protected void IndentProperty()
        {
            ImGui.Indent(ImGui.GetTreeNodeToLabelSpacing());
        }

        protected void UnindentProperty()
        {
            ImGui.Unindent(ImGui.GetTreeNodeToLabelSpacing());
        }

        protected void AlignProperty()
        {
            EditorGuiFieldsResolver.SetPropertyDefaultCursorPos();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Target that contains the property to override.</typeparam>
    /// <typeparam name="V">Type of the property.</typeparam>
    public abstract class PropertyDrawer<T, V> : PropertyDrawer
    {
        protected sealed internal override bool DrawProperty(Type type, string name, object target, in object valueIn,
                                                      out object valueOut, Func<bool> defaultPropertyDrawer)
        {
            var valueInV = valueIn != null ? (V)valueIn : default;
            return OnDrawProperty(type, name, target, ref valueInV, out valueOut, defaultPropertyDrawer);
        }

        internal virtual protected bool OnDrawProperty(Type type, string name, object target, ref V valueIn,
                                                      out object valueOut, Func<bool> defaultPropertyDrawer)
        {
            valueOut = null;
            return defaultPropertyDrawer?.Invoke() ?? false;
        }
    }

    /// <summary>
    /// Use this attribute to specify the exact property name to override the drawing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PropertyDrawerAttribute : Attribute
    {
        internal string PropertyName { get; }
        private PropertyDrawerAttribute() { }
        public PropertyDrawerAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
