using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Bounds
    {
        [SerializedField] public vec3 Max;
        [SerializedField] public vec3 Min;
        [ShowFieldNoSerialize(true)] public vec3 Size => Max - Min;
        [ShowFieldNoSerialize(true)] public vec3 Center => (Min + Max) * 0.5f;
        [ShowFieldNoSerialize(true)] public vec3 Extents => (Max - Min) * 0.5f;

        public static readonly Bounds One = new()
        {
            Min = new vec3(-0.5f, -0.5f, -0.5f),
            Max = new vec3( 0.5f,  0.5f,  0.5f)
        };
        public static readonly Bounds One2D = new()
        {
            Min = new vec3(-0.5f, -0.5f, 0.0f),
            Max = new vec3( 0.5f,  0.5f, 0.0f)
        };
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