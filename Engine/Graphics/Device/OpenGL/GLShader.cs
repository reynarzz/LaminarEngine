using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if DESKTOP
using static OpenGL.GL;
#else
using static OpenGL.ES.GLES30;
#endif
namespace Engine.Graphics.OpenGL
{
    internal class GLShader : GLGfxResource<ShaderDescriptor>
    {
        private readonly Dictionary<string, int> _uniformLocations;
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

            uint vertId = CompileShader(GL_VERTEX_SHADER, descriptor.VertexSource, descriptor.VertName);

            if (vertId == 0)
                return false;

            uint fragId = CompileShader(GL_FRAGMENT_SHADER, descriptor.FragmentSource, descriptor.FragName);

            if (fragId == 0)
                return false;

            glAttachShader(Handle, vertId);
            glAttachShader(Handle, fragId);

            glLinkProgram(Handle);

            glDeleteShader(vertId);
            glDeleteShader(fragId);

            return ValidateProgram(Handle);
        }

        private unsafe uint CompileShader(int shaderType, byte[] shaderSource, string name)
        {
            uint shaderId = glCreateShader(shaderType);
            string src = Encoding.UTF8.GetString(shaderSource);

            glShaderSource(shaderId, src);
            glCompileShader(shaderId);

#if DEBUG
            int result = GL_FALSE;
            glGetShaderiv(shaderId, GL_COMPILE_STATUS, &result);

            if (result == GL_FALSE)
            {
                int length = 0;
                glGetShaderiv(shaderId, GL_INFO_LOG_LENGTH, &length);
                var message = glGetShaderInfoLog(shaderId, length);

                Debug.Error($"failed to compile '{ShaderTypeName(shaderType)}' \n{message}, name: {name}");
                glDeleteShader(shaderId);

                return 0;
            }
#endif
            return shaderId;
        }

        private unsafe bool ValidateProgram(uint program)
        {
#if RELEASE
            return true;
#endif
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
            glValidateProgram(program);
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

        internal override void UpdateResource(ShaderDescriptor descriptor) 
        {
            Console.WriteLine("GL shader resource update");
            CreateResource(descriptor);
        }

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

            unsafe
            {
                fixed (int* v = value)
                    glUniform1iv(location, value.Length, v);
            }
        }

        internal void SetUniform(string name, vec2 value)
        {
            if (!GetLocation(name, out var location))
                return;

            unsafe
            {
                glUniform2fv(location, 1, &value.x);
            }
        }

        internal void SetUniform(string name, vec3 value)
        {
            if (!GetLocation(name, out var location))
                return;

            unsafe
            {
                glUniform3fv(location, 1, &value.x);
            }
        }

        internal void SetUniform(string name, vec4 value)
        {
            if (!GetLocation(name, out var location))
                return;

            unsafe
            {
                glUniform4fv(location, 1, &value.x);
            }
        }

        internal void SetUniform(string name, mat4 value)
        {
            if (!GetLocation(name, out var location))
                return;

            unsafe
            {
                glUniformMatrix4fv(location, 1, false, &value.c0.x);
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