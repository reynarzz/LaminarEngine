using System;
using System.Collections.Generic;

namespace Engine
{
    public class HermiteInterpolatedCurve<T> : AnimationCurveBase<T>
    {
        protected List<KeyFrameHermite<T>> Keyframes { get; } = new();
        public override float Duration => Keyframes[^1].Time;

        private readonly Func<T, T, T, T, float, T> _hermite;
        private readonly Func<T, T, T> _subtract;
        private readonly Func<T, float, T> _divide;
        private readonly Func<T, float, T> _multiply;

        public bool LoopInterpolate { get; set; } = true;

        public HermiteInterpolatedCurve(
            Func<T, T, T, T, float, T> hermite,
            Func<T, T, T> subtract,
            Func<T, float, T> divide,
            Func<T, float, T> multiply)
        {
            _hermite = hermite ?? throw new ArgumentNullException(nameof(hermite));
            _subtract = subtract ?? throw new ArgumentNullException(nameof(subtract));
            _divide = divide ?? throw new ArgumentNullException(nameof(divide));
            _multiply = multiply ?? throw new ArgumentNullException(nameof(multiply));
        }

        public void AddKeyFrame(float time, T value)
        {
            AddKeyFrame(time, value, default, default);
        }

        public void AddKeyFrame(float time, T value, T inTangent, T outTangent)
        {
            Keyframes.Add(new KeyFrameHermite<T>(time, value, inTangent, outTangent));
            SortKeyframes(Keyframes);
        }

        internal override T Evaluate(float time)
        {
            if (Keyframes.Count == 0)
            {
                return default;
            }

            if (Keyframes.Count == 1)
            {
                return Keyframes[0].Value;
            }

            float start = Keyframes[0].Time;
            float end = Keyframes[^1].Time;
            float duration = end - start;

            if (!LoopInterpolate)
            {
                if (time <= start)
                {
                    return Keyframes[0].Value;
                }
                if (time >= end)
                {
                    return Keyframes[^1].Value;
                }
            }
            else
            {
                // Wrap time into start end.
                time = ((time - start) % duration + duration) % duration + start;
            }

            for (int i = 0; i < Keyframes.Count - 1; i++)
            {
                var k1 = Keyframes[i];
                var k2 = Keyframes[i + 1];

                if (time >= k1.Time && time < k2.Time)
                {
                    float t = (time - k1.Time) / (k2.Time - k1.Time);
                    float dt = k2.Time - k1.Time;

                    return _hermite(
                        k1.Value,
                        _multiply(k1.OutTangent, dt),
                        _multiply(k2.InTangent, dt),
                        k2.Value,
                        t
                    );
                }
            }

            if (LoopInterpolate)
            {
                var kLast = Keyframes[^1];
                var kFirst = Keyframes[0];
                float t = (time - end + duration) / duration;

                return _hermite(kLast.Value, _multiply(kLast.OutTangent, duration),
                                _multiply(kFirst.InTangent, duration),
                                kFirst.Value,
                                t
                );
            }

            return Keyframes[^1].Value;
        }

        public void AutoSmoothTangents(bool cyclic = true)
        {
            int n = Keyframes.Count;
            if (n < 2)
            {
                return;
            }

            float start = Keyframes[0].Time;
            float end = Keyframes[^1].Time;
            float dur = end - start;

            for (int i = 0; i < n; i++)
            {
                int prevIdx = cyclic ? (i - 1 + n) % n : Math.Max(i - 1, 0);
                int nextIdx = cyclic ? (i + 1) % n : Math.Min(i + 1, n - 1);

                var prev = Keyframes[prevIdx];
                var next = Keyframes[nextIdx];
                var cur = Keyframes[i];

                float prevTime = prev.Time;
                float nextTime = next.Time;

                if (cyclic)
                {
                    if (prevIdx > i)
                    {
                        prevTime -= dur;
                    }

                    if (nextIdx < i)
                    {
                        nextTime += dur;
                    }
                }

                float dt = nextTime - prevTime;
                T dv = _subtract(next.Value, prev.Value);
                T tangent = dt != 0f ? _divide(dv, dt) : default;

                cur.InTangent = tangent;
                cur.OutTangent = tangent;
            }

            if (cyclic && n > 2)
            {
                Keyframes[0].InTangent = Keyframes[^1].OutTangent;
                Keyframes[0].OutTangent = Keyframes[^1].OutTangent;
            }
        }
    }
}
