using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SpriteCurve : ConstantCurve<Sprite>
    {
        public SpriteCurve() { }
        public SpriteCurve(float fps, params Sprite[] sprites)
        {
            var t = 1.0f / fps;

            for (int i = 0; i < sprites.Length; i++)
            {
                AddKeyFrame((float)i * t, sprites[i]);
            }
        }
    }
}
