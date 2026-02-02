using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class TriggerCondition : TransitionCondition
    {
        // Deserializer needs this
        public TriggerCondition() : base(null)
        {
        }
        public TriggerCondition(string property) : base(property) { }
        public override bool IsCondition(AnimatorParameters parameters)
        {
            return parameters.GetTrigger(Property);
        }
    }
}
