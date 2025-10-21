using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class FloatHermiteCurve : HermiteInterpolatedCurve<float>
    {
        private static readonly Func<float, float, float> Substract = (a, b) => a - b;
        private static readonly Func<float, float, float> Divide = (a, b) => a / b;
        private static readonly Func<float, float, float> Mul = (a, b) => a * b;
        public FloatHermiteCurve() : base(Mathf.Hermite, Substract, Divide, Mul) { }
    }
}
