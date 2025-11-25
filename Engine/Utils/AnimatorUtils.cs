using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimatorUtils
    {
        public static AnimationState AddState(Animator animator, string name, bool isLoop)
        {
            var clip = new AnimationClip(name, isLoop);
            var state = new AnimationState(name, clip);
            animator.AddState(state);
            return state;
        }
       
        public static AnimatorTransition AddTransition(AnimationState state, string toState, float blendTime, params TransitionCondition[] conditions)
        {
            var transition = new AnimatorTransition(toState, blendTime, conditions);
            state.AddTransition(transition);
            return transition;
        }

        public static AnimatorTransition AddTransition(AnimationState state, string toState, params TransitionCondition[] conditions)
        {
            return AddTransition(state, toState, 0, conditions);
        }
    }
}