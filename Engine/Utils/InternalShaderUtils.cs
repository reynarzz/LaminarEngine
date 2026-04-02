using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace Engine.Utils
{
    internal class InternalShaderUtils
    {
        internal readonly static string VertexShaderPink = @"
        #version 330 core

        layout(location = 0) in vec3 position;
        layout(location = 1) in vec2 uv;

        out vec2 fragUV;
        uniform mat4 uVP;

        void main() 
        {
            fragUV = uv;
            gl_Position = uVP * vec4(position, 1.0);
        }
        ";

        private static string fragmentPink = @"
        #version 330 core

        out vec4 color;

        void main()
        {
            color = vec4(1.0, 0.0, 1.0, 1.0); 
        }";

        internal static Shader GetShaderPink()
        {
            return new Shader(ShaderPlatformCleaner(VertexShaderPink), ShaderPlatformCleaner(fragmentPink), "Shader pink vert", "shader pink frag");
        }

        public static string ShaderPlatformCleaner(string shader)
        {
            StringBuilder _sb = new();

            if (shader.Contains("#version"))
            {
                var lines = shader.Split('\n');

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("#version"))
                    {
#if DESKTOP
                        lines[i] = "#version 330 core";
                        _sb.AppendLine(lines[i].TrimStart());
#elif MOBILE
                            lines[i] = "#version 300 es";
                            _sb.AppendLine(lines[i].TrimStart());
                            _sb.AppendLine("precision mediump float;");
#else
                         lines[i] = "#version 310";
                        _sb.AppendLine(lines[i]);
#endif
                    }
                    else
                    {
                        _sb.AppendLine(lines[i]);
                    }
                }
            }

            return _sb.ToString().TrimStart();
        }
    }
}
