using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum IntOp
    {
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
        Equal,
        NotEqual
    }

    public class IntCondition : TransitionCondition
    {
        [SerializedField] public IntOp Op { get; set; }
        [SerializedField] public int Compare { get; set; }
        // Deserializer needs this
        public IntCondition() : base(null)
        {
        }
        public IntCondition(string property, int compare, IntOp op) : base(property)
        {
            Op = op;
            Compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            var aValue = parameters.GetInt(Property);

            switch (Op)
            {
                case IntOp.LessThan:
                    return aValue < Compare;
                case IntOp.GreaterThan:
                    return aValue > Compare;
                case IntOp.LessThanOrEqual:
                    return aValue <= Compare;
                case IntOp.GreaterThanOrEqual:
                    return aValue >= Compare;
                case IntOp.Equal:
                    return aValue == Compare;
                case IntOp.NotEqual:
                    return aValue != Compare;
            }

            return false;
        }
    }
}
