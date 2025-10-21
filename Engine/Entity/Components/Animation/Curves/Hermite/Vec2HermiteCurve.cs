using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Vec2HermiteCurve : HermiteInterpolatedCurve<vec2>
    {
        private static readonly Func<vec2, vec2, vec2> Substract = (a, b) => a - b;
        private static readonly Func<vec2, float, vec2> Divide = (a, b) => a / b;
        public Vec2HermiteCurve() : base(Mathf.Hermite, Substract, Divide) { }
    }
}
