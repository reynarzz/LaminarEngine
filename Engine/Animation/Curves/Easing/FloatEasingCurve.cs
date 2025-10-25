using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class FloatEasingCurve : EasingCurve<float>
    {
        public FloatEasingCurve() : base(Easing.Apply)
        {
        }
    }
}
