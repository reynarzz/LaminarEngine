using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class HermiteInterpolatedCurve<T> : AnimationCurveBase<T>
    {
        private readonly Func<T, T, T, T, float, T> _hermite;

        public HermiteInterpolatedCurve(Func<T, T, T, T, float, T> hermite)
        {
            _hermite = hermite ?? throw new ArgumentNullException(nameof(hermite));
        }

        protected override T Evaluate(float time)
        {
            for (int i = 0; i < Keyframes.Count - 1; i++)
            {
                var k1 = (KeyFrameHermite<T>)Keyframes[i];
                var k2 = (KeyFrameHermite<T>)Keyframes[i + 1];
                if (time >= k1.Time && time < k2.Time)
                {
                    float t = (time - k1.Time) / (k2.Time - k1.Time);
                    return _hermite(k1.Value, k1.OutTangent, k2.InTangent, k2.Value, t);
                }
            }

            return ((KeyFrameHermite<T>)Keyframes[0]).Value;
        }
    }
}