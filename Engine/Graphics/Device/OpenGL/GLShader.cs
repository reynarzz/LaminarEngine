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
            GLHelpers.CheckGLError();

            if (vertId == 0)
                return false;

            uint fragId = CompileShader(GL_FRAGMENT_SHADER, descriptor.FragmentSource, descriptor.FragName);
            GLHelpers.CheckGLError();

            if (fragId == 0)
                return false;

            glAttachShader(Handle, vertId);
            GLHelpers.CheckGLError();

            glAttachShader(Handle, fragId);
            GLHelpers.CheckGLError();

            glLinkProgram(Handle);
            GLHelpers.CheckGLError();

            glDeleteShader(vertId);
            GLHelpers.CheckGLError();

            glDeleteShader(fragId);
            GLHelpers.CheckGLError();

            bool validated = true;

#if DEBUG && !IOS
            validated = ValidateProgram(Handle);
#endif
            return validated;
        }

        private unsafe uint CompileShader(int shaderType, byte[] shaderSource, string name)
        {
            uint shaderId = glCreateShader(shaderType);
            GLHelpers.CheckGLError();

            string src = Encoding.UTF8.GetString(shaderSource);

            glShaderSource(shaderId, src);
            GLHelpers.CheckGLError();

            glCompileShader(shaderId);
            GLHelpers.CheckGLError();

#if DEBUG
            int result = GL_FALSE;
            glGetShaderiv(shaderId, GL_COMPILE_STATUS, &result);
            GLHelpers.CheckGLError();

            if (result == GL_FALSE)
            {
                int length = 0;
                glGetShaderiv(shaderId, GL_INFO_LOG_LENGTH, &length);
                GLHelpers.CheckGLError();

                var message = glGetShaderInfoLog(shaderId, length);
                GLHelpers.CheckGLError();

                Debug.Error($"failed to compile '{ShaderTypeName(shaderType)}' \n{message}, name: {name}");
                glDeleteShader(shaderId);
                GLHelpers.CheckGLError();

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
            GLHelpers.CheckGLError();

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

        private readonly Dictionary<int, int> _intCache = new();
        private readonly Dictionary<int, uint> _uintCache = new();
        private readonly Dictionary<int, float> _floatCache = new();
        private readonly Dictionary<int, vec2> _vec2Cache = new();
        private readonly Dictionary<int, vec3> _vec3Cache = new();
        private readonly Dictionary<int, vec4> _vec4Cache = new();
        private readonly Dictionary<int, mat4> _mat4Cache = new();
        private readonly Dictionary<int, int[]> _intArrayCache = new();

        internal void SetUniform(string name, int value)
        {
            if (!GetLocation(name, out var location))
                return;

            if (_intCache.TryGetValue(location, out var oldValue) && oldValue == value)
                return;

            _intCache[location] = value;
            glUniform1i(location, value);
        }

        internal void SetUniform(string name, uint value)
        {
            if (!GetLocation(name, out var location))
                return;

            if (_uintCache.TryGetValue(location, out var oldValue) && oldValue == value)
                return;

            _uintCache[location] = value;
            glUniform1ui(location, value);
        }

        internal void SetUniformF(string name, float value)
        {
            if (!GetLocation(name, out var location))
                return;

            if (_floatCache.TryGetValue(location, out var oldValue) && BitwiseEqual(oldValue, value))
                return;

            _floatCache[location] = value;
            glUniform1f(location, value);
        }

        internal void SetUniform(string name, int[] value)
        {
            if (!GetLocation(name, out var location))
                return;

            if (_intArrayCache.TryGetValue(location, out var oldValue) && SequenceEqual(oldValue, value))
                return;

            _intArrayCache[location] = (int[])value.Clone();

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

            if (_vec2Cache.TryGetValue(location, out var oldValue) && BitwiseEqual(oldValue, value))
                return;

            _vec2Cache[location] = value;

            unsafe
            {
                glUniform2fv(location, 1, &value.x);
            }
        }

        internal void SetUniform(string name, vec3 value)
        {
            if (!GetLocation(name, out var location))
                return;

            if (_vec3Cache.TryGetValue(location, out var oldValue) && BitwiseEqual(oldValue, value))
                return;

            _vec3Cache[location] = value;

            unsafe
            {
                glUniform3fv(location, 1, &value.x);
            }
        }

        internal void SetUniform(string name, vec4 value)
        {
            if (!GetLocation(name, out var location))
                return;

            if (_vec4Cache.TryGetValue(location, out var oldValue) && BitwiseEqual(oldValue, value))
                return;

            _vec4Cache[location] = value;

            unsafe
            {
                glUniform4fv(location, 1, &value.x);
            }
        }

        internal void SetUniform(string name, mat4 value)
        {
            if (!GetLocation(name, out var location))
                return;

            if (_mat4Cache.TryGetValue(location, out var oldValue) && BitwiseEqual(oldValue, value))
                return;

            _mat4Cache[location] = value;

            unsafe
            {
                glUniformMatrix4fv(location, 1, false, &value.c0.x);
            }
        }

        private static unsafe bool BitwiseEqual(float a, float b)
        {
            return *(uint*)&a == *(uint*)&b;
        }

        private static unsafe bool BitwiseEqual(vec2 a, vec2 b)
        {
            return *(ulong*)&a == *(ulong*)&b;
        }

        private static unsafe bool BitwiseEqual(vec3 a, vec3 b)
        {
            ulong* pa = (ulong*)&a;
            ulong* pb = (ulong*)&b;

            return pa[0] == pb[0] && *(uint*)(pa + 1) == *(uint*)(pb + 1);
        }

        private static unsafe bool BitwiseEqual(vec4 a, vec4 b)
        {
            ulong* pa = (ulong*)&a;
            ulong* pb = (ulong*)&b;

            return pa[0] == pb[0] && pa[1] == pb[1];
        }

        private static unsafe bool BitwiseEqual(mat4 a, mat4 b)
        {
            ulong* pa = (ulong*)&a;
            ulong* pb = (ulong*)&b;

            return pa[0] == pb[0] &&
                   pa[1] == pb[1] &&
                   pa[2] == pb[2] &&
                   pa[3] == pb[3] &&
                   pa[4] == pb[4] &&
                   pa[5] == pb[5] &&
                   pa[6] == pb[6] &&
                   pa[7] == pb[7];
        }

        private static bool SequenceEqual(int[] a, int[] b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a == null || b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        // Tries to find the location for 'name', if found, the location will be cached.
        private bool GetLocation(string name, out int location)
        {
            if (_uniformLocations.TryGetValue(name, out location))
                return location >= 0;

            location = glGetUniformLocation(Handle, name);
            _uniformLocations.Add(name, location);

            return location >= 0;
        }
    }
}