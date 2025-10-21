using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimationState
    {
        public string Name { get; }
        public AnimationClip Clip { get; }
        private List<AnimatorTransition> _transitions = new();

        public AnimationState(string name, AnimationClip clip)
        {
            Name = name;
            Clip = clip;
        }

        public void AddTransition(AnimatorTransition transition)
        {
            _transitions.Add(transition);
        }

        public void RemoveTransition(AnimatorTransition transition)
        {
            _transitions.Remove(transition);
        }

        public AnimatorTransition CheckTransitions(AnimatorParameters parameters)
        {
            foreach (var transition in _transitions)
            {
                foreach (var condition in transition.Conditions)
                {
                    if (!condition.IsCondition(parameters))
                    {
                        return null;
                    }
                }

                return transition;
            }
            return null;
        }
    }
}
