using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class BoolCondition : TransitionCondition
    {
        [SerializedField] public bool Compare { get; set; }
        // Deserializer needs this
        public BoolCondition() : base(null)
        {
        }
        public BoolCondition(string property, bool compare) : base(property)
        {
            Compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            return parameters.GetBool(Property) == Compare;
        }
    }
}
