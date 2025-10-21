using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class QuatHermiteCurve : HermiteInterpolatedCurve<quat>
    {
        private static readonly Func<quat, quat, quat> Substract = (a, b) => a - b;
        private static readonly Func<quat, float, quat> Divide = (a, b) => a / b;
        public QuatHermiteCurve() : base(Mathf.Hermite, Substract, Divide) { }
    }
}
