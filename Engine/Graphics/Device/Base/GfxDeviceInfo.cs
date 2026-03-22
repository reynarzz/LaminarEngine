using Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    internal struct GfxDeviceInfo
    {
        public int MaxUniformsCount { get; internal set; }
        public int MaxSamples { get; internal set; }
        internal int MaxTexAccessInVertexShader { get; set; }
        internal int MaxHardwareTextureUnits { get; set; }
        internal string Vendor { get; set; }
        internal string Version { get; set; }
        internal string Device { get; set; }
        internal int MaxShaderTextureUnits => Mathf.Min(Consts.Graphics.MAX_TEXTURE_UNITS_SHADER, MaxHardwareTextureUnits);
    }
}