using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    internal class TextureDescriptor : IGfxResourceDescriptor
    {
        internal int Width { get; set; }
        internal int Height { get; set; }
        internal int XOffset { get; set; }
        internal int YOffset { get; set; }
        internal int Channels { get; set; }
        internal byte[] Buffer { get; set; }
        internal bool IsMultiSample { get; set; }
        internal int SamplesCount { get; set; }
        internal bool EnableMipMaps { get; set; }
        internal TextureMode Mode { get; set; } = TextureMode.Clamp;
        internal TextureFilter Filter { get; set; } = TextureFilter.Nearest;
    }
}
