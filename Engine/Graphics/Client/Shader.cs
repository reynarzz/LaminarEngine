using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Shader : AssetResourceBase
    {
        internal GfxResource NativeShader { get; }
        public Shader(string vertexCode, string fragmentCode) : this(vertexCode, fragmentCode, string.Empty, string.Empty) 
        {
          
        }

        public Shader(string vertexCode, string fragmentCode, string vertName, string fragName) : 
            base(string.Empty, Guid.NewGuid()) // TODO: load shaders from file.
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

        public static Shader FromPath(string vertex, string fragment)
        {
            var vertexCode = Assets.GetText(vertex).Text;
            var fragCode = Assets.GetText(fragment).Text;
            return new Shader(vertexCode, fragCode, System.IO.Path.GetFileName(vertex), System.IO.Path.GetFileName(fragment));
        }

        protected internal override void OnDestroy()
        {
            GfxDeviceManager.Current.DestroyResource(NativeShader);
        }
    }
}
