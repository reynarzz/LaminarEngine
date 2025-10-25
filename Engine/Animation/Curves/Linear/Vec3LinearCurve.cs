using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Vec3LinearCurve : LinearInterpolatedCurve<vec3>
    {
        public Vec3LinearCurve() : base(Mathf.Lerp) { }
    }
}
