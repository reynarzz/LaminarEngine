using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ColorLinearCurve : LinearInterpolatedCurve<Color>
    {
        public ColorLinearCurve() : base(Color.Lerp) { }
    }
}
