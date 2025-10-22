using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class TrueCondition : TransitionCondition
    {
        public TrueCondition() : base(null)
        {
        }

        public override bool IsCondition(AnimatorParameters parameters)
        {
            return true;
        }
    }
}
