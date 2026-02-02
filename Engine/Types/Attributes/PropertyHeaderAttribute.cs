using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PropertyHeaderAttribute : Attribute
    {
        internal string HeaderText { get; }
        public PropertyHeaderAttribute(string header)
        {
            HeaderText = header;
        }
    }
}
