using Engine;
using Engine.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedTypes
{
    public struct ShaderUniform
    {
        public string Name { get;  set; }
        public UniformType Type { get; set; }
        public int ElementCount { get; set; }
    }
}
