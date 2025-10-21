using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimatorTransition
    {
        public string ToState { get; }
        public Func<AnimatorParameters, bool>[] Conditions { get; }
        public float BlendTime { get; }

        public AnimatorTransition(string toState, float blendTime, Func<AnimatorParameters, bool>[] conditions)
        {
            ToState = toState;
            Conditions = conditions;
            BlendTime = blendTime;
        }

        public AnimatorTransition(string toState, float blendTime, Func<AnimatorParameters, bool> condition) 
            : this(toState, blendTime, [condition])
        {
        }

        public AnimatorTransition(string toState, Func<AnimatorParameters, bool> condition)
            : this(toState, 0, [condition])
        {
        }
    }
}
