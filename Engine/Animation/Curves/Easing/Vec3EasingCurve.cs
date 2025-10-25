using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Vec3EasingCurve : EasingCurve<vec3>
    {
        public Vec3EasingCurve() : base(Easing.Apply)
        {
        }
    }
}
