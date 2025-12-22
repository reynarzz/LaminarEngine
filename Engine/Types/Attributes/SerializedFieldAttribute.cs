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

        public SerializedFieldAttribute()
        {
        }

        public SerializedFieldAttribute(string fieldName)
        {
            CustomFieldName = fieldName;
        }
    }
}