using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class HermiteInterpolatedCurve<T> : AnimationCurveBase<T>
    {
        protected List<KeyFrameHermite<T>> Keyframes { get; } = new();
        public override float Duration => Keyframes[^1].Time;

        private readonly Func<T, T, T, T, float, T> _hermite;
        private readonly Func<T, T, T> _subtract;
        private readonly Func<T, float, T> _divide;

        public void AddKeyFrame(float time, T value)
        {
            AddKeyFrame(time, value, default, default);
        }

        public void AddKeyFrame(float time, T value, T inTangent, T outTangent)
        {
            Keyframes.Add(new KeyFrameHermite<T>(time, value, inTangent, outTangent));
            SortKeyframes(Keyframes);
        }

        public HermiteInterpolatedCurve(Func<T, T, T, T, float, T> hermite, Func<T, T, T> substract, Func<T, float, T> divide)
        {
            _hermite = hermite ?? throw new ArgumentNullException(nameof(hermite));
            _subtract = substract ?? throw new ArgumentNullException(nameof(substract));
            _divide = divide ?? throw new ArgumentNullException(nameof(divide));
        }
        
        internal override T Evaluate(float time)
        {
            if (Keyframes.Count == 0)
                return default;

            if (time >= Keyframes[^1].Time)
                return Keyframes[^1].Value;

            for (int i = 0; i < Keyframes.Count - 1; i++)
            {
                var k1 = Keyframes[i];
                var k2 = Keyframes[i + 1];
                if (time >= k1.Time && time < k2.Time)
                {
                    float t = (time - k1.Time) / (k2.Time - k1.Time);
                    return _hermite(k1.Value, k1.OutTangent, k2.InTangent, k2.Value, t);
                }
            }

            return Keyframes[0].Value;
        }

        /// <summary>
        /// Call at the end when finished adding all the keyframes.
        /// </summary>
        public void AutoSmoothTangents()
        {
            if (Keyframes.Count < 2)
                return;

            for (int i = 0; i < Keyframes.Count; i++)
            {
                var prevKey = (i > 0) ? Keyframes[i - 1] : Keyframes[i];
                var nextKey = (i < Keyframes.Count - 1) ? Keyframes[i + 1] : Keyframes[i];

                float deltaTime = nextKey.Time - prevKey.Time;

                T deltaValue = _subtract(nextKey.Value, prevKey.Value);
                T tangent = (deltaTime != 0f) ? _divide(deltaValue, deltaTime) : default;

                var currentKey = Keyframes[i];
                currentKey.InTangent = tangent;
                currentKey.OutTangent = tangent;
            }
        }
    }
}