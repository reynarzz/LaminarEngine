using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
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
        [SerializedField] public bool HasErrors { get; set; }
        [SerializedField] public ShaderStage Stage { get; set; }
        [SerializedField] public byte[] Shader { get; set; }
        [SerializedField] public ShaderUniform[] Uniforms { get; set; }
    }
}
