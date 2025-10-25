using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Vec2EasingCurve : EasingCurve<vec2>
    {
        public Vec2EasingCurve() : base(Easing.Apply)
        {
        }
    }
}
