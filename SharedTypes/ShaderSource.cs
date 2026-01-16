using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    internal class ShaderSource
    {
        public byte[] Shader { get; set; }
        public ShaderUniform[] Uniforms { get; set; }
    }
}
