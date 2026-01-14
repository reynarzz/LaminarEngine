using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class DefaultKeyframeCurve<T> : AnimationCurveBase<T>
    {
        [SerializedField] protected List<Keyframe<T>> Keyframes { get; private set; } = new();
        public override float Duration => Keyframes.Count > 0 ? Keyframes[^1].Time : 0;
        public override void AddKeyFrame(float time, T value)
        {
            Keyframes.Add(new Keyframe<T>(time, value));
            SortKeyframes(Keyframes);
        }
    }
}
