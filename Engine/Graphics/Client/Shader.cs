using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Shader : EObject
    {
        internal GfxResource NativeShader { get; }
        public Shader(string vertexCode, string fragmentCode) : this(vertexCode, fragmentCode, string.Empty, string.Empty)
        {
          
        }

        public Shader(string vertexCode, string fragmentCode, string vertName, string fragName)
        {
            var shaderDescriptor = new ShaderDescriptor()
            {
                VertexSource = Encoding.UTF8.GetBytes(vertexCode),
                FragmentSource = Encoding.UTF8.GetBytes(fragmentCode),
                VertName = vertName,
                FragName = fragName
            };
         
            NativeShader = GfxDeviceManager.Current.CreateShader(shaderDescriptor);
        }

        protected internal override void OnDestroy()
        {
            GfxDeviceManager.Current.DestroyResource(NativeShader);
        }
    }
}
