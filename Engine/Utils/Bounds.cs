using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct Bounds
    {
        public vec3 Max;
        public vec3 Min;
        public vec3 Size => Max - Min;
        public vec3 Center => (Min + Max) * 0.5f;
        public vec3 Extents => (Max - Min) * 0.5f;

        internal static Bounds GetInitialized()
        {
            return new Bounds()
            {
                Min = vec3.One * float.MaxValue,
                Max = vec3.One * float.MinValue
            };
        }
    }
}