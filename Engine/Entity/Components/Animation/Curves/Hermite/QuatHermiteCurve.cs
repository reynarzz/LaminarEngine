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
        public QuatHermiteCurve() : base(Mathf.Hermite) { }
    }
}
