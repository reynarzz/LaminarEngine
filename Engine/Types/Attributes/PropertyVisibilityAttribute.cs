using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class PropertyVisibilityAttribute : Attribute
    {
        public string CustomFieldName { get; } = string.Empty;
        public bool IsReadOnly { get; }
        public PropertyVisibilityAttribute() { }
        public PropertyVisibilityAttribute(bool isReadOnly) : this(string.Empty, isReadOnly) { }
        public PropertyVisibilityAttribute(string fieldName) : this(fieldName, false) { }
        public PropertyVisibilityAttribute(string fieldName, bool isReadOnly)
        {
            CustomFieldName = fieldName;
            IsReadOnly = isReadOnly;
        }
    }
}
