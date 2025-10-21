using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ColorEasingCurve : EasingCurve<Color>
    {
        public ColorEasingCurve() : base(Easing.Apply)
        {
        }
    }
}
