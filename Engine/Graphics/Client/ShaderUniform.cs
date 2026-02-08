using Engine;
using Engine.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct ShaderUniform
    {
        [SerializedField] public string Name { get;  set; }
        [SerializedField] public UniformType Type { get; set; }
        [SerializedField] public int ElementCount { get; set; }
    }
}
