using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum FloatOp
    {
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
    }

    public class FloatCondition : TransitionCondition
    {
        [SerializedField] public FloatOp Op { get; set; }
        [SerializedField] public float Compare { get; set; }
        // Deserializer needs this
        private FloatCondition() : base(null)
        {
        }
        public FloatCondition(string property, float compare, FloatOp op) : base(property)
        {
            Op = op;
            Compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            var aValue = parameters.GetFloat(Property);

            switch (Op)
            {
                case FloatOp.LessThan:
                    return aValue < Compare;
                case FloatOp.GreaterThan:
                    return aValue > Compare;
                case FloatOp.LessThanOrEqual:
                    return aValue <= Compare;
                case FloatOp.GreaterThanOrEqual:
                    return aValue >= Compare;
            }

            return false;
        }
    }
}
