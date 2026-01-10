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
        [SerializedField] private FloatOp _op;
        [SerializedField] private float _compare;
        public FloatCondition(string property, float compare, FloatOp op) : base(property)
        {
            _op = op;
            _compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            var aValue = parameters.GetFloat(Property);

            switch (_op)
            {
                case FloatOp.LessThan:
                    return aValue < _compare;
                case FloatOp.GreaterThan:
                    return aValue > _compare;
                case FloatOp.LessThanOrEqual:
                    return aValue <= _compare;
                case FloatOp.GreaterThanOrEqual:
                    return aValue >= _compare;
            }

            return false;
        }
    }
}
