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
            if (Keyframes.Count == 0)
            {
                return default;
            }
            if (time >= Keyframes[^1].Time)
            {
                return Keyframes[^1].Value;
            }

            for (int i = 0; i < Keyframes.Count - 1; i++)
            {
                if (time >= Keyframes[i].Time && time < Keyframes[i + 1].Time)
                {
                    return Keyframes[i].Value;
                }
            }

            return Keyframes[0].Value;
        }
    }
}
