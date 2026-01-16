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
    public class ShaderUniform
    {
        [JsonProperty] public string Name { get; internal set; }
        [JsonProperty] public UniformType Type { get; internal set; }
        [JsonProperty] public int ElementCount { get; set; }
    }
}
