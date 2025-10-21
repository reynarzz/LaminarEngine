using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class DefaultKeyframeCurve<T> : AnimationCurveBase<T>
    {
        protected List<KeyFrameBase<T>> Keyframes { get; } = new();
        public override float Duration => Keyframes.Count > 0 ? Keyframes[^1].Time : 0;
        public void AddKeyFrame(float time, T value)
        {
            Keyframes.Add(new KeyFrameBase<T>(time, value));
            SortKeyframes(Keyframes);
        }
    }
}
