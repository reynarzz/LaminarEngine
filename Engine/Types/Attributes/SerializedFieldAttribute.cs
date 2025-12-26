using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SerializedFieldAttribute : Attribute
    {
        public string CustomFieldName { get; } = string.Empty;
        public bool IsReadOnly { get; }
        public SerializedFieldAttribute()
        {
        }

        public SerializedFieldAttribute(bool isReadOnly) : this(string.Empty, isReadOnly)
        {
        }
        public SerializedFieldAttribute(string fieldName) : this(fieldName, false)
        {

        }
        public SerializedFieldAttribute(string fieldName, bool isReadOnly)
        {
            CustomFieldName = fieldName;
            IsReadOnly = isReadOnly;
        }
    }
}