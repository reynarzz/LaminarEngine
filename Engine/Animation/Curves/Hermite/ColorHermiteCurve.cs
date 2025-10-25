using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class ColorHermiteCurve : HermiteInterpolatedCurve<Color>
    {
        private static readonly Func<Color, Color, Color> Substract = (a, b) => new Color(a.R - b.R, a.G - b.G, a.B - b.B, a.A - b.A);
        private static readonly Func<Color, float, Color> Divide = (v, s) => new Color(v.R / s, v.G / s, v.B / s, v.A / s);
        private static readonly Func<Color, float, Color> Mul = (v, s) => new Color(v.R * s, v.G * s, v.B * s, v.A * s);
        public ColorHermiteCurve() : base(Color.Hermite, Substract, Divide, Mul) { }
    }
}