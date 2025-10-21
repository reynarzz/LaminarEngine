using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class FloatCurve : AnimationCurveBase<float>
    {
        public override float Evaluate(float time)
        {
            if (Keyframes.Count == 0) return 0f;

            if (time >= Keyframes[^1].Time)
            {
                return Keyframes[^1].Value;
            }

            for (int i = 0; i < Keyframes.Count - 1; i++)
            {
                var k1 = Keyframes[i];
                var k2 = Keyframes[i + 1];
                if (time >= k1.Time && time < k2.Time)
                {
                    float t = (time - k1.Time) / (k2.Time - k1.Time);
                    return Mathf.Lerp(k1.Value, k2.Value, t);
                }
            }

            return Keyframes[0].Value;
        }
    }

}
