using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    internal class RenderTargetDescriptor : IGfxResourceDescriptor
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsMultiSample { get; set; }
        public int SamplesCount { get; set; }
        public TextureDescriptor ColorTextureDescriptor { get; } = new(); 
    }
}
