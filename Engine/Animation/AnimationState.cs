using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimationState
    {
        [SerializedField] public string Name { get; set; }
        [SerializedField] public AnimationClip Clip { get; set; }
        [SerializedField] private List<AnimatorTransition> _transitions = new();
        internal IReadOnlyList<AnimatorTransition> Transitions => _transitions;

        public AnimationState()
        {
        }
        public AnimationState(string name, AnimationClip clip)
        {
            Name = name;
            Clip = clip;
        }

        public void AddTransition(AnimatorTransition transition)
        {
            _transitions.Add(transition);
        }

        public void AddTransition(Span<AnimatorTransition> transitions)
        {
            _transitions.AddRange(transitions);
        }

        public void RemoveTransition(AnimatorTransition transition)
        {
            _transitions.Remove(transition);
        }

        public AnimatorTransition CheckTransitions(AnimatorParameters parameters)
        {
            foreach (var transition in _transitions)
            {
                bool allConditionsMet = true;
                foreach (var condition in transition.Conditions)
                {
                    if (!condition.IsCondition(parameters))
                    {
                        allConditionsMet = false;
                        break;
                    }
                }

                if (allConditionsMet)
                {
                    return transition;
                }
            }
            return null;
        }
    }
}
