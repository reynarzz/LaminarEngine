using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class AnimationCurveBase
    {
        public abstract float Duration { get; }
        // internal abstract CurveEvaluatedResult Evaluate(float time);
    }
    public abstract class AnimationCurveBase<T> : AnimationCurveBase
    {
         internal abstract T Evaluate(float time);
        protected void SortKeyframes<TKey>(List<TKey> keyframes) where TKey : IKeyFrame
        {
            keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));
        }

        public abstract void AddKeyFrame(float time, T value);
    }

    public struct CurveEvaluatedResult
    {
        public vec2 Vec2 { get; set; }
        public vec3 Vec3 { get; set; }
        public quat Quat { get; set; }
        public float Float { get; set; }
        public int Int { get; set; }
        public bool Bool { get; set; }
        public object Object { get; set; }
    }
}