using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class AnimationCurveBase<T>
    {
        public abstract float Duration { get; }
        internal abstract T Evaluate(float time);
        protected void SortKeyframes<TKey>(List<TKey> keyframes) where TKey : IKeyFrame
        {
            keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));
        }

        public abstract void AddKeyFrame(float time, T value);
    }
}