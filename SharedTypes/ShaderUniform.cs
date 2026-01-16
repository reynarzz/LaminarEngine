using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    public class ShaderUniform
    {
        public string Name { get; internal set; }
        public UniformType Type { get; internal set; }
        public int ElementCount { get; set; }
    }
}
