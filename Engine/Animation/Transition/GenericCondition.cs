using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class GenericCondition : TransitionCondition
    {
        private readonly Func<AnimatorParameters, bool> _compare;
        // Deserializer needs this
        private GenericCondition() : base(null)
        {
        }
        public GenericCondition(Func<AnimatorParameters, bool> compare) : base(null)
        {
            _compare = compare;
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            return _compare(parameters);
        }
    }
}
