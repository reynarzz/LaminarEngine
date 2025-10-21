using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal class EventKeyFrame : KeyFrameBase<Action>
    {
        internal bool Raised { get; set; }
        internal EventKeyFrame(float time, Action value) : base(time, value)
        {
        }
    }

    public class EventCurve : AnimationCurveBase<Action>
    {
        private List<EventKeyFrame> Keyframes { get; } = new();
        public override float Duration => Keyframes[^1].Time;
        public void AddKeyFrame(float time, Action value)
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
                Keyframes[i].Raised = false;
            }
        }
    }
}