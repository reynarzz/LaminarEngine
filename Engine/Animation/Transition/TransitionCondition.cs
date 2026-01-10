using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class TransitionCondition 
    {
        [SerializedField] internal string Property { get; set; }

        protected TransitionCondition(string property)
        {
            Property = property;
        }

        public abstract bool IsCondition(AnimatorParameters parameters); 
    }
}
