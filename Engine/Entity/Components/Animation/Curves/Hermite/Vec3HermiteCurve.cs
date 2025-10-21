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
        public Vec3HermiteCurve() : base(Mathf.Hermite) { }
    }
}
