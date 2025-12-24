using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    internal interface ICamera
    {
        public mat4 Projection { get; }
        public mat4 ViewMatrix { get; }
        public RenderTexture RenderTexture { get; }
    }
}
