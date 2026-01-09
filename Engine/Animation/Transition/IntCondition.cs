using Newtonsoft.Json;
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
        [SerializedField, HideFromInspector, JsonProperty] private IntOp _op;
        [SerializedField, HideFromInspector, JsonProperty] private float _compare;
        public IntCondition(string property, int compare, IntOp op) : base(property)
        {
            _op = op;
            _compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            var aValue = parameters.GetInt(Property);

            switch (_op)
            {
                case IntOp.LessThan:
                    return aValue < _compare;
                case IntOp.GreaterThan:
                    return aValue > _compare;
                case IntOp.LessThanOrEqual:
                    return aValue <= _compare;
                case IntOp.GreaterThanOrEqual:
                    return aValue >= _compare;
                case IntOp.Equal:
                    return aValue == _compare;
                case IntOp.NotEqual:
                    return aValue != _compare;
            }

            return false;
        }
    }
}
