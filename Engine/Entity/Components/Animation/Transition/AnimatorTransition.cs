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
        public TransitionCondition[] Conditions { get; }
        public float BlendTime { get; set; }

        public AnimatorTransition(AnimatorTransition copy)
        {
            ToState = copy.ToState;
            BlendTime = copy.BlendTime;
            Conditions = copy.Conditions;
        }

        public AnimatorTransition(string toState, float blendTime, TransitionCondition[] conditions)
        {
            ToState = toState;
            Conditions = conditions;
            BlendTime = blendTime;
        }

        public AnimatorTransition(string toState, float blendTime, TransitionCondition condition) 
            : this(toState, blendTime, [condition])
        {
        }

        public AnimatorTransition(string toState, TransitionCondition condition)
            : this(toState, 0, [condition])
        {
        }

        public AnimatorTransition(string toState, TransitionCondition[] condition)
            : this(toState, 0, condition)
        {
        }

        public AnimatorTransition(string toState)
            : this(toState, 0, new TrueCondition())
        {
        }
    }
}
