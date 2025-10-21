using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Vec2LinearCurve : LinearInterpolatedCurve<vec2>
    {
        public Vec2LinearCurve() : base(Mathf.Lerp) { }
    }
}
