using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ShowFieldNoSerialize : PropertyVisibilityAttribute
    {
        public ShowFieldNoSerialize() { }
        public ShowFieldNoSerialize(bool isReadOnly) : this(string.Empty, isReadOnly) { }
        public ShowFieldNoSerialize(string fieldName) : this(fieldName, false) { }
        public ShowFieldNoSerialize(string fieldName, bool isReadOnly) : base(fieldName, isReadOnly) { }
    }
}
