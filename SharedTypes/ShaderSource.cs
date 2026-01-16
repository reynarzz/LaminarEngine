using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    internal enum ShaderStage
    {
        Vertex,
        TessControl,
        TessEvaluation,
        Geometry,
        Fragment,
        Compute,
        Raygen,
        Intersect,
        AnyHit,
        ClosestHit,
        Miss,
        Callable,
        Task,
        Mesh
    }

    internal class ShaderSource
    {
        public bool HasErrors { get; set; }
        public ShaderStage Stage { get; set; }
        public byte[] Shader { get; set; }
        public ShaderUniform[] Uniforms { get; set; }
    }
}
