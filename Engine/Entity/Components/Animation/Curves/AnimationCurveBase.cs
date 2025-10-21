using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class AnimationCurveBase<T>
    {
        protected List<KeyFrameBase<T>> Keyframes { get; } = new();
        internal T EvaluateTime(float time)
        {
            if (Keyframes.Count == 0)
            {
                return default;
            }
            if (time >= Keyframes[^1].Time)
            {
                return Keyframes[^1].Value;
            }
            return EvaluateTime(time);
        }

        protected abstract T Evaluate(float time);

        public float Duration => Keyframes[^1].Time;
        public virtual void AddKeyFrame(float time, T value)
        {
            Keyframes.Add(new KeyFrameBase<T>(time, value));
            Keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
    }
}