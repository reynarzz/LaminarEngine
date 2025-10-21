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
        public abstract T Evaluate(float time);
        public float Duration => Keyframes[^1].Time;
        public virtual void AddKeyFrame(float time, T sprite)
        {
            Keyframes.Add(new KeyFrameBase<T>(time, sprite));
            Keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));
        }
    }
}