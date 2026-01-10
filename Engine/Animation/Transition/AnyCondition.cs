using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnyCondition : TransitionCondition
    {
        [SerializedField] private TransitionCondition[] _conditions;
        public AnyCondition(TransitionCondition[] conditions) : base(null)
        {
            _conditions = conditions;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            if(_conditions == null || _conditions.Length == 0)
                return false;

            for (int i = 0; i < _conditions.Length; i++)
            {
                if (_conditions[i].IsCondition(parameters))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
