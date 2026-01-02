using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RequireComponentAttribute : Attribute
    {
        public Type[] RequiredComponents { get; }
        private RequireComponentAttribute() { }
        public RequireComponentAttribute(params Type[] components)
        {
            RequiredComponents = components;
        }
    }
}
