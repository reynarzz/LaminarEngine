using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.GL;

namespace Engine.Graphics.OpenGL
{
    internal class GLShader : GLGfxResource<ShaderDescriptor>
    {
        private readonly Dictionary<string, int> _uniformLocations;
        private readonly static float[] _mat4Arr = new float[16];
        private readonly static float[] _vec4Arr = new float[4];
        private readonly static float[] _vec3Arr = new float[3];
        private readonly static float[] _vec2Arr = new float[2];
        public GLShader() : base(glCreateProgram, glDeleteProgram, glUseProgram)
        {
            _uniformLocations = new Dictionary<string, int>(StringComparer.Ordinal);
        }

        protected override bool CreateResource(ShaderDescriptor descriptor)
        {
            if (descriptor.VertexSource == null || descriptor.FragmentSource == null)
            {
                Debug.Error("Shaders sources are invalid");
                return false;
            }

            uint vertId = CompileShader(GL_VERTEX_SHADER, descriptor.VertexSource);

            if (vertId == 0)
                return false;

            uint fragId = CompileShader(GL_FRAGMENT_SHADER, descriptor.FragmentSource);

            if (fragId == 0)
                return false;

            glAttachShader(Handle, vertId);
            glAttachShader(Handle, fragId);

            glLinkProgram(Handle);

            glDeleteShader(vertId);
            glDeleteShader(fragId);


            return ValidateProgram(Handle);
        }

        private unsafe uint CompileShader(int shaderType, byte[] shaderSource)
        {
            uint shaderId = glCreateShader(shaderType);
            string src = Encoding.UTF8.GetString(shaderSource);

            glShaderSource(shaderId, src);
            glCompileShader(shaderId);

            int result = GL_FALSE;

            glGetShaderiv(shaderId, GL_COMPILE_STATUS, &result);

            if (result == GL_FALSE)
            {
                int length = 0;
                glGetShaderiv(shaderId, GL_INFO_LOG_LENGTH, &length);
                var message = glGetShaderInfoLog(shaderId, length);

                Debug.Error($"failed to compile '{ShaderTypeName(shaderType)}' \n{message}");
                glDeleteShader(shaderId);

                return 0;
            }

            return shaderId;
        }

        private unsafe bool ValidateProgram(uint program)
        {
            int linkStatus;
            glGetProgramiv(program, GL_LINK_STATUS, &linkStatus);
            if (linkStatus == GL_FALSE)
            {
                int length;
                glGetProgramiv(program, GL_INFO_LOG_LENGTH, &length);
                var log = glGetProgramInfoLog(program, length);
                Debug.Error($"Program linking failed: {log}");
                return false;
            }
#if DEBUG
            glValidateProgram(program);
#endif
            int status;
            glGetProgramiv(program, GL_VALIDATE_STATUS, &status);
            if (status == GL_FALSE)
            {
                int logLength;
                glGetProgramiv(program, GL_INFO_LOG_LENGTH, &logLength);

                var log = glGetProgramInfoLog(program, logLength);
                Debug.Error($"Program validation failed: {log}");

                return false;
            }

            return true;
        }

        private static string ShaderTypeName(int shaderType) => shaderType switch
        {
            GL_VERTEX_SHADER => "vertex",
            GL_FRAGMENT_SHADER => "fragment",
            _ => "unknown"
        };

        internal override void UpdateResource(ShaderDescriptor descriptor) { }

        internal void SetUniform(string name, int value)
        {
            if (!GetLocation(name, out var location))
                return;

            glUniform1i(location, value);
        }

        internal void SetUniform(string name, uint value)
        {
            if (!GetLocation(name, out var location))
                return;

            glUniform1ui(location, value);
        }

        internal void SetUniformF(string name, float value)
        {
            if (!GetLocation(name, out var location))
                return;

            glUniform1f(location, value);
        }

        internal void SetUniform(string name, int[] value)
        {
            if (!GetLocation(name, out var location))
                return;

            glUniform1iv(location, value.Length, value);
        }

        internal void SetUniform(string name, vec2 value)
        {
            if (!GetLocation(name, out var location))
                return;

            value.to_array(_vec2Arr);
            glUniform2fv(location, 1, _vec2Arr);
        }

        internal void SetUniform(string name, vec3 value)
        {
            if (!GetLocation(name, out var location))
                return;

            value.to_array(_vec3Arr);
            glUniform3fv(location, 1, _vec3Arr);
        }

        internal void SetUniform(string name, vec4 value)
        {
            if (!GetLocation(name, out var location))
                return;

            value.to_array(_vec4Arr);
            glUniform4fv(location, 1, _vec4Arr);
        }

        internal void SetUniform(string name, mat4 value)
        {
            if (!GetLocation(name, out var location))
                return;

            unsafe
            {
                value.to_array(_mat4Arr);
                fixed (float* m = &_mat4Arr[0])
                {
                    glUniformMatrix4fv(location, 1, false, m);
                }
            }
        }

        // Tries to find the location for 'name', if found, the location will be cached.
        private bool GetLocation(string name, out int location)
        {
            location = -1;
            if (_uniformLocations.TryGetValue(name, out location))
            {
                return location >= 0;
            }
            location = glGetUniformLocation(Handle, name);
            _uniformLocations.Add(name, location);
            return false;
        }
    }
}