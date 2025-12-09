using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    internal class ShaderDescriptor : IGfxResourceDescriptor
    {
        public string VertName { get; set; }
        public string FragName { get; set; }
        public byte[] VertexSource { get; set; }
        public byte[] FragmentSource { get; set; }
    }
}
