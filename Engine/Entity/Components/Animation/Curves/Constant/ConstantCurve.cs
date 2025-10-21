using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ConstantCurve<T> : DefaultKeyframeCurve<T>
    {
        internal override T Evaluate(float time)
        {
            int count = Keyframes.Count;
            if (count == 0)
                return default;

            if (count == 1 || time <= Keyframes[0].Time)
                return Keyframes[0].Value;

            if (time >= Keyframes[^1].Time)
                return Keyframes[^1].Value;

            for (int i = 0; i < count - 1; i++)
            {
                var k1 = Keyframes[i];
                var k2 = Keyframes[i + 1];

                if (time >= k1.Time && time <= k2.Time)
                    return k1.Value;
            }

            return Keyframes[^1].Value;
        }
    }
}
