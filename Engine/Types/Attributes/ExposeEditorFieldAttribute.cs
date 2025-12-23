using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ExposeEditorFieldAttribute : Attribute
    {
        public string CustomFieldName { get; } = string.Empty;
        public bool IsReadOnly { get; }
        public ExposeEditorFieldAttribute()
        {
        }

        public ExposeEditorFieldAttribute(bool isReadOnly) : this(string.Empty, isReadOnly)
        {
        }
        public ExposeEditorFieldAttribute(string fieldName) : this(fieldName, false)
        {

        }
        public ExposeEditorFieldAttribute(string fieldName, bool isReadOnly)
        {
            CustomFieldName = fieldName;
            IsReadOnly = isReadOnly;
        }
    }
}