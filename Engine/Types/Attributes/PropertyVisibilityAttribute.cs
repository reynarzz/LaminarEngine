using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class PropertyVisibilityAttribute : Attribute
    {
        internal string CustomFieldName { get; } = string.Empty;
        internal bool IsReadOnly { get; }
        internal PropertyVisibilityAttribute() { }
        internal PropertyVisibilityAttribute(bool isReadOnly) : this(string.Empty, isReadOnly) { }
        internal PropertyVisibilityAttribute(string fieldName) : this(fieldName, false) { }
        internal PropertyVisibilityAttribute(string fieldName, bool isReadOnly)
        {
            CustomFieldName = fieldName;
            IsReadOnly = isReadOnly;
        }
    }
}