using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class EventCurve : AnimationCurveBase<Action>
    {
        private struct EventKeyFrame : IKeyFrame<Action>
        {
            internal bool Raised; // NOTE: Do not serialize.
            // TODO: Check if the target is part of the actor that has the Animator that has the animationController,
            // if so, it can serialize the function name? or If is not found create a event dispatcher: stateName + eventTime.
            [SerializedField] public Action Value { get; set; }

            [SerializedField] public float Time { get; set; }
            public EventKeyFrame()
            {
            }
            internal EventKeyFrame(float time, Action value)
            {
                Time = time;
                Value = value;
            }
        }

        [SerializedField] private List<EventKeyFrame> Keyframes { get; set; } = new();
        public override float Duration => Keyframes.Count > 0 ? Keyframes[^1].Time : 0;
        public override void AddKeyFrame(float time, Action value)
        {
            Keyframes.Add(new EventKeyFrame(time, value));
            SortKeyframes(Keyframes);
        }

        internal override Action Evaluate(float time)
        {
            if (Keyframes.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < Keyframes.Count; i++)
            {
                if (time >= Keyframes[i].Time)
                {
                    var key = Keyframes[i];
                    if (!key.Raised)
                    {
                        key.Raised = true;
                        Keyframes[i] = key;
                        key.Value?.Invoke();
                    }
                }
            }

            return null;
        }

        internal void Restart()
        {
            for (int i = 0; i < Keyframes.Count; i++)
            {
                var key = Keyframes[i];
                key.Raised = false;
                Keyframes[i] = key;
            }
        }
    }
}