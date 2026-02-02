using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ShowMethodInEditorAttribute : Attribute
    {
        public bool SameLineNextFunction { get; }
        public ShowMethodInEditorAttribute(bool sameLineNextFunction = false)
        {
            SameLineNextFunction = sameLineNextFunction;
        }
    }
}
