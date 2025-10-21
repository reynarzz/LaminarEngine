using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum FloatConditionOp
    {
        LessThan,
        GreaterThan,
        LessThanOrEqual,
        GreaterThanOrEqual,
    }

    public class FloatCondition : TransitionCondition
    {
        private readonly FloatConditionOp _op;
        private readonly float _compare;
        public FloatCondition(string property, float compare, FloatConditionOp op) : base(property)
        {
            _op = op;
            _compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            var aValue = parameters.GetFloat(Property);

            switch (_op)
            {
                case FloatConditionOp.LessThan:
                    return aValue < _compare;
                case FloatConditionOp.GreaterThan:
                    return aValue > _compare;
                case FloatConditionOp.LessThanOrEqual:
                    return aValue <= _compare;
                case FloatConditionOp.GreaterThanOrEqual:
                    return aValue >= _compare;
            }

            return false;
        }
    }
}
