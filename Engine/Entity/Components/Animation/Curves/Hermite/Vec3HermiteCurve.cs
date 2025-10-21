using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Vec3HermiteCurve : HermiteInterpolatedCurve<vec3>
    {
        private static readonly Func<vec3, vec3, vec3> Substract = (a, b) => a - b;
        private static readonly Func<vec3, float, vec3> Divide = (a, b) => a / b;
        public Vec3HermiteCurve() : base(Mathf.Hermite, Substract, Divide) { }
    }
}
