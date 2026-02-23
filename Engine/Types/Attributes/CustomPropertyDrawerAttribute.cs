using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public abstract class CustomPropertyDrawerAttribute : Attribute
    {
        internal abstract IPropertyDrawer GetDrawer();
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class CustomPropertyDrawerAttribute<T> : CustomPropertyDrawerAttribute where T: class, IPropertyDrawer, new()
    {
        private readonly static T _propertyDrawer = new T();
        public CustomPropertyDrawerAttribute() { }
        internal override T GetDrawer()
        {
            return _propertyDrawer;
        }
    }

    public interface IPropertyDrawer
    {
        internal protected bool Draw(string propertyName, object target, in object valueIn, out object valueOut);
    } 
}
