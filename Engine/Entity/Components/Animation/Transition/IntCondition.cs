using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum IntConditionOp
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
        private readonly IntConditionOp _op;
        private readonly float _compare;
        public IntCondition(string property, int compare, IntConditionOp op) : base(property)
        {
            _op = op;
            _compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            var aValue = (int)Math.Round(parameters.GetFloat(Property));

            switch (_op)
            {
                case IntConditionOp.LessThan:
                    return aValue < _compare;
                case IntConditionOp.GreaterThan:
                    return aValue > _compare;
                case IntConditionOp.LessThanOrEqual:
                    return aValue <= _compare;
                case IntConditionOp.GreaterThanOrEqual:
                    return aValue >= _compare;
                case IntConditionOp.Equal:
                    return aValue == _compare;
                case IntConditionOp.NotEqual:
                    return aValue != _compare;
            }

            return false;
        }
    }
}
