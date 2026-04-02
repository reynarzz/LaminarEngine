using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace OpenGL.ES
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static unsafe partial class GLES30
    {
#if ANDROID
        public const string LIB = "libGLESv2.so";
#elif IOS || __IOS__
        public const string LIB = "__Internal";
#else
        public const string LIB = "GLESv2";
#endif

        // ── String helpers ────────────────────────────────────────────────────

        private static string PtrToStringUtf8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return string.Empty;
            var length = 0;
            while (Marshal.ReadByte(ptr, length) != 0) length++;
            var buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }

        private static string PtrToStringUtf8(IntPtr ptr, int length)
        {
            if (ptr == IntPtr.Zero || length <= 0) return string.Empty;
            var buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }

        // ── GL ES 2.0 functions ───────────────────────────────────────────────

        [LibraryImport(LIB, EntryPoint = "glActiveTexture")]
        public static partial void glActiveTexture(int texture);

        [LibraryImport(LIB, EntryPoint = "glAttachShader")]
        public static partial void glAttachShader(uint program, uint shader);

        [LibraryImport(LIB, EntryPoint = "glBindAttribLocation", StringMarshalling = StringMarshalling.Utf8)]
        public static partial void glBindAttribLocation(uint program, uint index, string name);

        [LibraryImport(LIB, EntryPoint = "glBindBuffer")]
        public static partial void glBindBuffer(int target, uint buffer);

        [LibraryImport(LIB, EntryPoint = "glBindFramebuffer")]
        public static partial void glBindFramebuffer(int target, uint framebuffer);

        [LibraryImport(LIB, EntryPoint = "glBindRenderbuffer")]
        public static partial void glBindRenderbuffer(int target, uint renderbuffer);

        public static void glBindRenderbuffer(uint renderbuffer)
            => glBindRenderbuffer(GL_RENDERBUFFER, renderbuffer);

        [LibraryImport(LIB, EntryPoint = "glBindTexture")]
        public static partial void glBindTexture(int target, uint texture);

        [LibraryImport(LIB, EntryPoint = "glBlendColor")]
        public static partial void glBlendColor(float red, float green, float blue, float alpha);

        [LibraryImport(LIB, EntryPoint = "glBlendEquation")]
        public static partial void glBlendEquation(int mode);

        [LibraryImport(LIB, EntryPoint = "glBlendEquationSeparate")]
        public static partial void glBlendEquationSeparate(int modeRGB, int modeAlpha);

        [LibraryImport(LIB, EntryPoint = "glBlendFunc")]
        public static partial void glBlendFunc(int sfactor, int dfactor);

        [LibraryImport(LIB, EntryPoint = "glBlendFuncSeparate")]
        public static partial void glBlendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);

        [LibraryImport(LIB, EntryPoint = "glBufferData")]
        public static partial void glBufferData(int target, IntPtr size, void* data, int usage);

        [LibraryImport(LIB, EntryPoint = "glBufferSubData")]
        public static partial void glBufferSubData(int target, IntPtr offset, IntPtr size, void* data);

        [LibraryImport(LIB, EntryPoint = "glCheckFramebufferStatus")]
        public static partial int glCheckFramebufferStatus(int target);

        [LibraryImport(LIB, EntryPoint = "glClear")]
        public static partial void glClear(uint mask);

        [LibraryImport(LIB, EntryPoint = "glClearColor")]
        public static partial void glClearColor(float red, float green, float blue, float alpha);

        [LibraryImport(LIB, EntryPoint = "glClearDepthf")]
        public static partial void glClearDepthf(float d);

        [LibraryImport(LIB, EntryPoint = "glClearStencil")]
        public static partial void glClearStencil(int s);

        [LibraryImport(LIB, EntryPoint = "glColorMask")]
        public static partial void glColorMask(
            [MarshalAs(UnmanagedType.U1)] bool red,
            [MarshalAs(UnmanagedType.U1)] bool green,
            [MarshalAs(UnmanagedType.U1)] bool blue,
            [MarshalAs(UnmanagedType.U1)] bool alpha);

        [LibraryImport(LIB, EntryPoint = "glCompileShader")]
        public static partial void glCompileShader(uint shader);

        [LibraryImport(LIB, EntryPoint = "glCompressedTexImage2D")]
        public static partial void glCompressedTexImage2D(int target, int level, int internalformat,
            int width, int height, int border, int imageSize, void* data);

        [LibraryImport(LIB, EntryPoint = "glCompressedTexSubImage2D")]
        public static partial void glCompressedTexSubImage2D(int target, int level,
            int xoffset, int yoffset, int width, int height, int format, int imageSize, void* data);

        [LibraryImport(LIB, EntryPoint = "glCopyTexImage2D")]
        public static partial void glCopyTexImage2D(int target, int level, int internalformat,
            int x, int y, int width, int height, int border);

        [LibraryImport(LIB, EntryPoint = "glCopyTexSubImage2D")]
        public static partial void glCopyTexSubImage2D(int target, int level,
            int xoffset, int yoffset, int x, int y, int width, int height);

        [LibraryImport(LIB, EntryPoint = "glCreateProgram")]
        public static partial uint glCreateProgram();

        [LibraryImport(LIB, EntryPoint = "glCreateShader")]
        public static partial uint glCreateShader(int type);

        [LibraryImport(LIB, EntryPoint = "glCullFace")]
        public static partial void glCullFace(int mode);

        [LibraryImport(LIB, EntryPoint = "glDeleteBuffers")]
        public static partial void glDeleteBuffers(int n, uint* buffers);

        public static void glDeleteBuffer(uint buffer)
            => glDeleteBuffers(1, &buffer);

        [LibraryImport(LIB, EntryPoint = "glDeleteFramebuffers")]
        public static partial void glDeleteFramebuffers(int n, uint* framebuffers);

        public static void glDeleteFramebuffer(uint framebuffer)
            => glDeleteFramebuffers(1, &framebuffer);

        [LibraryImport(LIB, EntryPoint = "glDeleteProgram")]
        public static partial void glDeleteProgram(uint program);

        [LibraryImport(LIB, EntryPoint = "glDeleteRenderbuffers")]
        public static partial void glDeleteRenderbuffers(int n, uint* renderbuffers);

        public static void glDeleteRenderbuffer(uint renderbuffer)
            => glDeleteRenderbuffers(1, &renderbuffer);

        [LibraryImport(LIB, EntryPoint = "glDeleteShader")]
        public static partial void glDeleteShader(uint shader);

        [LibraryImport(LIB, EntryPoint = "glDeleteTextures")]
        public static partial void glDeleteTextures(int n, uint* textures);

        public static void glDeleteTexture(uint texture)
            => glDeleteTextures(1, &texture);

        [LibraryImport(LIB, EntryPoint = "glDepthFunc")]
        public static partial void glDepthFunc(int func);

        [LibraryImport(LIB, EntryPoint = "glDepthMask")]
        public static partial void glDepthMask([MarshalAs(UnmanagedType.U1)] bool flag);

        [LibraryImport(LIB, EntryPoint = "glDepthRangef")]
        public static partial void glDepthRangef(float n, float f);

        [LibraryImport(LIB, EntryPoint = "glDetachShader")]
        public static partial void glDetachShader(uint program, uint shader);

        [LibraryImport(LIB, EntryPoint = "glDisable")]
        public static partial void glDisable(int cap);

        [LibraryImport(LIB, EntryPoint = "glDisableVertexAttribArray")]
        public static partial void glDisableVertexAttribArray(uint index);

        [LibraryImport(LIB, EntryPoint = "glDrawArrays")]
        public static partial void glDrawArrays(int mode, int first, int count);

        [LibraryImport(LIB, EntryPoint = "glDrawElements")]
        public static partial void glDrawElements(int mode, int count, int type, void* indices);

        [LibraryImport(LIB, EntryPoint = "glEnable")]
        public static partial void glEnable(int cap);

        [LibraryImport(LIB, EntryPoint = "glEnableVertexAttribArray")]
        public static partial void glEnableVertexAttribArray(uint index);

        [LibraryImport(LIB, EntryPoint = "glFinish")]
        public static partial void glFinish();

        [LibraryImport(LIB, EntryPoint = "glFlush")]
        public static partial void glFlush();

        [LibraryImport(LIB, EntryPoint = "glFramebufferRenderbuffer")]
        public static partial void glFramebufferRenderbuffer(int target, int attachment,
            int renderbuffertarget, uint renderbuffer);

        [LibraryImport(LIB, EntryPoint = "glFramebufferTexture2D")]
        public static partial void glFramebufferTexture2D(int target, int attachment,
            int textarget, uint texture, int level);

        [LibraryImport(LIB, EntryPoint = "glFrontFace")]
        public static partial void glFrontFace(int mode);

        [LibraryImport(LIB, EntryPoint = "glGenBuffers")]
        public static partial void glGenBuffers(int n, uint* buffers);

        public static uint glGenBuffer()
        {
            uint id;
            glGenBuffers(1, &id);
            return id;
        }

        [LibraryImport(LIB, EntryPoint = "glGenerateMipmap")]
        public static partial void glGenerateMipmap(int target);

        [LibraryImport(LIB, EntryPoint = "glGenFramebuffers")]
        public static partial void glGenFramebuffers(int n, uint* framebuffers);

        public static uint glGenFramebuffer()
        {
            uint id = 0;
            glGenFramebuffers(1, &id);
            return id;
        }

        [LibraryImport(LIB, EntryPoint = "glGenRenderbuffers")]
        public static partial void glGenRenderbuffers(int n, uint* renderbuffers);

        public static uint glGenRenderbuffer()
        {
            uint id = 0;
            glGenRenderbuffers(1, &id);
            return id;
        }

        [LibraryImport(LIB, EntryPoint = "glGenTextures")]
        public static partial void glGenTextures(int n, uint* textures);

        public static uint glGenTexture()
        {
            uint id = 0;
            glGenTextures(1, &id);
            return id;
        }

        [LibraryImport(LIB, EntryPoint = "glGetActiveAttrib")]
        public static partial void glGetActiveAttrib(uint program, uint index, int bufSize,
            int* length, int* size, int* type, byte* name);

        [LibraryImport(LIB, EntryPoint = "glGetActiveUniform")]
        public static partial void glGetActiveUniform(uint program, uint index, int bufSize,
            int* length, int* size, int* type, byte* name);

        [LibraryImport(LIB, EntryPoint = "glGetAttachedShaders")]
        public static partial void glGetAttachedShaders(uint program, int maxCount,
            int* count, uint* shaders);

        [LibraryImport(LIB, EntryPoint = "glGetAttribLocation", StringMarshalling = StringMarshalling.Utf8)]
        public static partial int glGetAttribLocation(uint program, string name);

        [LibraryImport(LIB, EntryPoint = "glGetBooleanv")]
        public static partial void glGetBooleanv(int pname, byte* data);

        [LibraryImport(LIB, EntryPoint = "glGetBufferParameteriv")]
        public static partial void glGetBufferParameteriv(int target, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetError")]
        public static partial int glGetError();
        public static int GetError() => glGetError();

        [LibraryImport(LIB, EntryPoint = "glGetFloatv")]
        public static partial void glGetFloatv(int pname, float* data);

        [LibraryImport(LIB, EntryPoint = "glGetFramebufferAttachmentParameteriv")]
        public static partial void glGetFramebufferAttachmentParameteriv(int target,
            int attachment, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetIntegerv")]
        public static partial void glGetIntegerv(int pname, int* data);

        [LibraryImport(LIB, EntryPoint = "glGetProgramiv")]
        public static partial void glGetProgramiv(uint program, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetProgramInfoLog")]
        public static partial void glGetProgramInfoLog(uint program, int bufSize,
            int* length, byte* infoLog);

        public static string glGetProgramInfoLog(uint program, int bufSize = 1024)
        {
            var buffer = Marshal.AllocHGlobal(bufSize);
            try
            {
                int length;
                glGetProgramInfoLog(program, bufSize, &length, (byte*)buffer.ToPointer());
                return PtrToStringUtf8(buffer, length);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        [LibraryImport(LIB, EntryPoint = "glGetRenderbufferParameteriv")]
        public static partial void glGetRenderbufferParameteriv(int target, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetShaderiv")]
        public static partial void glGetShaderiv(uint shader, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetShaderInfoLog")]
        public static partial void glGetShaderInfoLog(uint shader, int bufSize,
            int* length, byte* infoLog);

        public static string glGetShaderInfoLog(uint shader, int bufSize = 1024)
        {
            var buffer = Marshal.AllocHGlobal(bufSize);
            try
            {
                int length;
                glGetShaderInfoLog(shader, bufSize, &length, (byte*)buffer.ToPointer());
                return PtrToStringUtf8(buffer, length);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        [LibraryImport(LIB, EntryPoint = "glGetShaderPrecisionFormat")]
        public static partial void glGetShaderPrecisionFormat(int shadertype, int precisiontype,
            int* range, int* precision);

        [LibraryImport(LIB, EntryPoint = "glGetShaderSource")]
        public static partial void glGetShaderSource(uint shader, int bufSize,
            int* length, byte* source);

        [LibraryImport(LIB, EntryPoint = "glGetString")]
        private static partial IntPtr _glGetString(int name);

        public static string glGetString(int name)
            => PtrToStringUtf8(_glGetString(name));

        [LibraryImport(LIB, EntryPoint = "glGetTexParameterfv")]
        public static partial void glGetTexParameterfv(int target, int pname, float* @params);

        [LibraryImport(LIB, EntryPoint = "glGetTexParameteriv")]
        public static partial void glGetTexParameteriv(int target, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetUniformfv")]
        public static partial void glGetUniformfv(uint program, int location, float* @params);

        [LibraryImport(LIB, EntryPoint = "glGetUniformiv")]
        public static partial void glGetUniformiv(uint program, int location, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetUniformLocation", StringMarshalling = StringMarshalling.Utf8)]
        public static partial int glGetUniformLocation(uint program, string name);

        [LibraryImport(LIB, EntryPoint = "glGetVertexAttribfv")]
        public static partial void glGetVertexAttribfv(uint index, int pname, float* @params);

        [LibraryImport(LIB, EntryPoint = "glGetVertexAttribiv")]
        public static partial void glGetVertexAttribiv(uint index, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetVertexAttribPointerv")]
        public static partial void glGetVertexAttribPointerv(uint index, int pname, void** pointer);

        [LibraryImport(LIB, EntryPoint = "glHint")]
        public static partial void glHint(int target, int mode);

        [LibraryImport(LIB, EntryPoint = "glIsBuffer")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsBuffer(uint buffer);

        [LibraryImport(LIB, EntryPoint = "glIsEnabled")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsEnabled(int cap);

        [LibraryImport(LIB, EntryPoint = "glIsFramebuffer")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsFramebuffer(uint framebuffer);

        [LibraryImport(LIB, EntryPoint = "glIsProgram")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsProgram(uint program);

        [LibraryImport(LIB, EntryPoint = "glIsRenderbuffer")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsRenderbuffer(uint renderbuffer);

        [LibraryImport(LIB, EntryPoint = "glIsShader")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsShader(uint shader);

        [LibraryImport(LIB, EntryPoint = "glIsTexture")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsTexture(uint texture);

        [LibraryImport(LIB, EntryPoint = "glLineWidth")]
        public static partial void glLineWidth(float width);

        [LibraryImport(LIB, EntryPoint = "glLinkProgram")]
        public static partial void glLinkProgram(uint program);

        [LibraryImport(LIB, EntryPoint = "glPixelStorei")]
        public static partial void glPixelStorei(int pname, int param);

        [LibraryImport(LIB, EntryPoint = "glPolygonOffset")]
        public static partial void glPolygonOffset(float factor, float units);

        [LibraryImport(LIB, EntryPoint = "glReadPixels")]
        public static partial void glReadPixels(int x, int y, int width, int height,
            int format, int type, void* pixels);

        public static void glReadPixels(int x, int y, int width, int height,
            int format, int type, IntPtr pixels)
            => glReadPixels(x, y, width, height, format, type, pixels.ToPointer());

        [LibraryImport(LIB, EntryPoint = "glReleaseShaderCompiler")]
        public static partial void glReleaseShaderCompiler();

        [LibraryImport(LIB, EntryPoint = "glRenderbufferStorage")]
        public static partial void glRenderbufferStorage(int target, int internalformat,
            int width, int height);

        [LibraryImport(LIB, EntryPoint = "glSampleCoverage")]
        public static partial void glSampleCoverage(float value,
            [MarshalAs(UnmanagedType.U1)] bool invert);

        [LibraryImport(LIB, EntryPoint = "glScissor")]
        public static partial void glScissor(int x, int y, int width, int height);

        [LibraryImport(LIB, EntryPoint = "glShaderBinary")]
        public static partial void glShaderBinary(int count, uint* shaders, int binaryformat,
            void* binary, int length);

        [LibraryImport(LIB, EntryPoint = "glShaderSource")]
        private static partial void glShaderSource_(uint shader, int count,
            byte** strings, int* lengths);

        public static void glShaderSource(uint shader, string src)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(src);
            fixed (byte* pUtf8 = utf8)
            {
                byte* ptr = pUtf8;
                int len = utf8.Length;   // no null terminator in the length
                glShaderSource_(shader, 1, &ptr, &len);
            }
        }

        [LibraryImport(LIB, EntryPoint = "glStencilFunc")]
        public static partial void glStencilFunc(int func, int @ref, uint mask);

        [LibraryImport(LIB, EntryPoint = "glStencilFuncSeparate")]
        public static partial void glStencilFuncSeparate(int face, int func, int @ref, uint mask);

        [LibraryImport(LIB, EntryPoint = "glStencilMask")]
        public static partial void glStencilMask(uint mask);

        [LibraryImport(LIB, EntryPoint = "glStencilMaskSeparate")]
        public static partial void glStencilMaskSeparate(int face, uint mask);

        [LibraryImport(LIB, EntryPoint = "glStencilOp")]
        public static partial void glStencilOp(int fail, int zfail, int zpass);

        [LibraryImport(LIB, EntryPoint = "glStencilOpSeparate")]
        public static partial void glStencilOpSeparate(int face, int sfail, int dpfail, int dppass);

        [LibraryImport(LIB, EntryPoint = "glTexImage2D")]
        [UnmanagedCallConv(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        public static partial void glTexImage2D(int target, int level, int internalformat,
            int width, int height, int border, int format, int type, void* data);

        [LibraryImport(LIB, EntryPoint = "glTexParameterf")]
        public static partial void glTexParameterf(int target, int pname, float param);

        [LibraryImport(LIB, EntryPoint = "glTexParameterfv")]
        public static partial void glTexParameterfv(int target, int pname, float* @params);

        [LibraryImport(LIB, EntryPoint = "glTexParameteri")]
        public static partial void glTexParameteri(int target, int pname, int param);

        [LibraryImport(LIB, EntryPoint = "glTexParameteriv")]
        public static partial void glTexParameteriv(int target, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glTexSubImage2D")]
        public static partial void glTexSubImage2D(int target, int level,
            int xoffset, int yoffset, int width, int height, int format, int type, void* data);

        [LibraryImport(LIB, EntryPoint = "glUniform1f")]
        public static partial void glUniform1f(int location, float v0);

        [LibraryImport(LIB, EntryPoint = "glUniform1fv")]
        public static partial void glUniform1fv(int location, int count, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniform1i")]
        public static partial void glUniform1i(int location, int v0);

        [LibraryImport(LIB, EntryPoint = "glUniform1iv")]
        public static partial void glUniform1iv(int location, int count, int* value);

        [LibraryImport(LIB, EntryPoint = "glUniform2f")]
        public static partial void glUniform2f(int location, float v0, float v1);

        [LibraryImport(LIB, EntryPoint = "glUniform2fv")]
        public static partial void glUniform2fv(int location, int count, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniform2i")]
        public static partial void glUniform2i(int location, int v0, int v1);

        [LibraryImport(LIB, EntryPoint = "glUniform2iv")]
        public static partial void glUniform2iv(int location, int count, int* value);

        [LibraryImport(LIB, EntryPoint = "glUniform3f")]
        public static partial void glUniform3f(int location, float v0, float v1, float v2);

        [LibraryImport(LIB, EntryPoint = "glUniform3fv")]
        public static partial void glUniform3fv(int location, int count, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniform3i")]
        public static partial void glUniform3i(int location, int v0, int v1, int v2);

        [LibraryImport(LIB, EntryPoint = "glUniform3iv")]
        public static partial void glUniform3iv(int location, int count, int* value);

        [LibraryImport(LIB, EntryPoint = "glUniform4f")]
        public static partial void glUniform4f(int location, float v0, float v1, float v2, float v3);

        [LibraryImport(LIB, EntryPoint = "glUniform4fv")]
        public static partial void glUniform4fv(int location, int count, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniform4i")]
        public static partial void glUniform4i(int location, int v0, int v1, int v2, int v3);

        [LibraryImport(LIB, EntryPoint = "glUniform4iv")]
        public static partial void glUniform4iv(int location, int count, int* value);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix2fv")]
        public static partial void glUniformMatrix2fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix3fv")]
        public static partial void glUniformMatrix3fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix4fv")]
        public static partial void glUniformMatrix4fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glUseProgram")]
        public static partial void glUseProgram(uint program);

        [LibraryImport(LIB, EntryPoint = "glValidateProgram")]
        public static partial void glValidateProgram(uint program);

        [LibraryImport(LIB, EntryPoint = "glVertexAttrib1f")]
        public static partial void glVertexAttrib1f(uint index, float x);

        [LibraryImport(LIB, EntryPoint = "glVertexAttrib1fv")]
        public static partial void glVertexAttrib1fv(uint index, float* v);

        [LibraryImport(LIB, EntryPoint = "glVertexAttrib2f")]
        public static partial void glVertexAttrib2f(uint index, float x, float y);

        [LibraryImport(LIB, EntryPoint = "glVertexAttrib2fv")]
        public static partial void glVertexAttrib2fv(uint index, float* v);

        [LibraryImport(LIB, EntryPoint = "glVertexAttrib3f")]
        public static partial void glVertexAttrib3f(uint index, float x, float y, float z);

        [LibraryImport(LIB, EntryPoint = "glVertexAttrib3fv")]
        public static partial void glVertexAttrib3fv(uint index, float* v);

        [LibraryImport(LIB, EntryPoint = "glVertexAttrib4f")]
        public static partial void glVertexAttrib4f(uint index, float x, float y, float z, float w);

        [LibraryImport(LIB, EntryPoint = "glVertexAttrib4fv")]
        public static partial void glVertexAttrib4fv(uint index, float* v);

        [LibraryImport(LIB, EntryPoint = "glVertexAttribPointer")]
        public static partial void glVertexAttribPointer(uint index, int size, int type,
            [MarshalAs(UnmanagedType.U1)] bool normalized, int stride, void* pointer);

        public static void glVertexAttribPointer(uint index, int size, int type,
            bool normalized, int stride, IntPtr pointer)
            => glVertexAttribPointer(index, size, type, normalized, stride, pointer.ToPointer());

        [LibraryImport(LIB, EntryPoint = "glViewport")]
        public static partial void glViewport(int x, int y, int width, int height);

        // ── GL ES 3.0 functions ───────────────────────────────────────────────

        [LibraryImport(LIB, EntryPoint = "glReadBuffer")]
        public static partial void glReadBuffer(int mode);

        [LibraryImport(LIB, EntryPoint = "glDrawRangeElements")]
        public static partial void glDrawRangeElements(int mode, uint start, uint end,
            int count, int type, void* indices);

        [LibraryImport(LIB, EntryPoint = "glTexImage3D")]
        public static partial void glTexImage3D(int target, int level, int internalformat,
            int width, int height, int depth, int border, int format, int type, void* data);

        [LibraryImport(LIB, EntryPoint = "glTexSubImage3D")]
        public static partial void glTexSubImage3D(int target, int level,
            int xoffset, int yoffset, int zoffset,
            int width, int height, int depth, int format, int type, void* data);

        [LibraryImport(LIB, EntryPoint = "glCopyTexSubImage3D")]
        public static partial void glCopyTexSubImage3D(int target, int level,
            int xoffset, int yoffset, int zoffset, int x, int y, int width, int height);

        [LibraryImport(LIB, EntryPoint = "glCompressedTexImage3D")]
        public static partial void glCompressedTexImage3D(int target, int level, int internalformat,
            int width, int height, int depth, int border, int imageSize, void* data);

        [LibraryImport(LIB, EntryPoint = "glCompressedTexSubImage3D")]
        public static partial void glCompressedTexSubImage3D(int target, int level,
            int xoffset, int yoffset, int zoffset,
            int width, int height, int depth, int format, int imageSize, void* data);

        [LibraryImport(LIB, EntryPoint = "glGenQueries")]
        public static partial void glGenQueries(int n, uint* ids);

        public static uint glGenQuery()
        {
            uint id = 0;
            glGenQueries(1, &id);
            return id;
        }

        [LibraryImport(LIB, EntryPoint = "glDeleteQueries")]
        public static partial void glDeleteQueries(int n, uint* ids);

        public static void glDeleteQuery(uint id)
            => glDeleteQueries(1, &id);

        [LibraryImport(LIB, EntryPoint = "glIsQuery")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsQuery(uint id);

        [LibraryImport(LIB, EntryPoint = "glBeginQuery")]
        public static partial void glBeginQuery(int target, uint id);

        [LibraryImport(LIB, EntryPoint = "glEndQuery")]
        public static partial void glEndQuery(int target);

        [LibraryImport(LIB, EntryPoint = "glGetQueryiv")]
        public static partial void glGetQueryiv(int target, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetQueryObjectuiv")]
        public static partial void glGetQueryObjectuiv(uint id, int pname, uint* @params);

        [LibraryImport(LIB, EntryPoint = "glUnmapBuffer")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glUnmapBuffer(int target);

        [LibraryImport(LIB, EntryPoint = "glGetBufferPointerv")]
        public static partial void glGetBufferPointerv(int target, int pname, void** @params);

        [LibraryImport(LIB, EntryPoint = "glDrawBuffers")]
        public static partial void glDrawBuffers(int n, int* bufs);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix2x3fv")]
        public static partial void glUniformMatrix2x3fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix3x2fv")]
        public static partial void glUniformMatrix3x2fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix2x4fv")]
        public static partial void glUniformMatrix2x4fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix4x2fv")]
        public static partial void glUniformMatrix4x2fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix3x4fv")]
        public static partial void glUniformMatrix3x4fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glUniformMatrix4x3fv")]
        public static partial void glUniformMatrix4x3fv(int location, int count,
            [MarshalAs(UnmanagedType.U1)] bool transpose, float* value);

        [LibraryImport(LIB, EntryPoint = "glBlitFramebuffer")]
        public static partial void glBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1,
            int dstX0, int dstY0, int dstX1, int dstY1, uint mask, int filter);

        [LibraryImport(LIB, EntryPoint = "glRenderbufferStorageMultisample")]
        public static partial void glRenderbufferStorageMultisample(int target, int samples,
            int internalformat, int width, int height);

        [LibraryImport(LIB, EntryPoint = "glFramebufferTextureLayer")]
        public static partial void glFramebufferTextureLayer(int target, int attachment,
            uint texture, int level, int layer);

        [LibraryImport(LIB, EntryPoint = "glMapBufferRange")]
        public static partial void* glMapBufferRange(int target, IntPtr offset,
            IntPtr length, uint access);

        [LibraryImport(LIB, EntryPoint = "glFlushMappedBufferRange")]
        public static partial void glFlushMappedBufferRange(int target,
            IntPtr offset, IntPtr length);

        [LibraryImport(LIB, EntryPoint = "glBindVertexArray")]
        public static partial void glBindVertexArray(uint array);

        [LibraryImport(LIB, EntryPoint = "glDeleteVertexArrays")]
        public static partial void glDeleteVertexArrays(int n, uint* arrays);

        public static void glDeleteVertexArray(uint array)
            => glDeleteVertexArrays(1, &array);

        [LibraryImport(LIB, EntryPoint = "glGenVertexArrays")]
        public static partial void glGenVertexArrays(int n, uint* arrays);

        public static uint glGenVertexArray()
        {
            uint id = 0;
            glGenVertexArrays(1, &id);
            return id;
        }

        [LibraryImport(LIB, EntryPoint = "glIsVertexArray")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsVertexArray(uint array);

        [LibraryImport(LIB, EntryPoint = "glGetIntegeri_v")]
        public static partial void glGetIntegeri_v(int target, uint index, int* data);

        [LibraryImport(LIB, EntryPoint = "glBeginTransformFeedback")]
        public static partial void glBeginTransformFeedback(int primitiveMode);

        [LibraryImport(LIB, EntryPoint = "glEndTransformFeedback")]
        public static partial void glEndTransformFeedback();

        [LibraryImport(LIB, EntryPoint = "glBindBufferRange")]
        public static partial void glBindBufferRange(int target, uint index, uint buffer,
            IntPtr offset, IntPtr size);

        [LibraryImport(LIB, EntryPoint = "glBindBufferBase")]
        public static partial void glBindBufferBase(int target, uint index, uint buffer);

        [LibraryImport(LIB, EntryPoint = "glTransformFeedbackVaryings",
            StringMarshalling = StringMarshalling.Utf8)]
        public static partial void glTransformFeedbackVaryings(uint program, int count,
            string[] varyings, int bufferMode);

        [LibraryImport(LIB, EntryPoint = "glGetTransformFeedbackVarying")]
        public static partial void glGetTransformFeedbackVarying(uint program, uint index,
            int bufSize, int* length, int* size, int* type, byte* name);

        [LibraryImport(LIB, EntryPoint = "glVertexAttribIPointer")]
        public static partial void glVertexAttribIPointer(uint index, int size, int type,
            int stride, void* pointer);

        public static void glVertexAttribIPointer(uint index, int size, int type,
            int stride, IntPtr pointer)
            => glVertexAttribIPointer(index, size, type, stride, pointer.ToPointer());

        [LibraryImport(LIB, EntryPoint = "glGetVertexAttribIiv")]
        public static partial void glGetVertexAttribIiv(uint index, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetVertexAttribIuiv")]
        public static partial void glGetVertexAttribIuiv(uint index, int pname, uint* @params);

        [LibraryImport(LIB, EntryPoint = "glVertexAttribI4i")]
        public static partial void glVertexAttribI4i(uint index, int x, int y, int z, int w);

        [LibraryImport(LIB, EntryPoint = "glVertexAttribI4ui")]
        public static partial void glVertexAttribI4ui(uint index, uint x, uint y, uint z, uint w);

        [LibraryImport(LIB, EntryPoint = "glVertexAttribI4iv")]
        public static partial void glVertexAttribI4iv(uint index, int* v);

        [LibraryImport(LIB, EntryPoint = "glVertexAttribI4uiv")]
        public static partial void glVertexAttribI4uiv(uint index, uint* v);

        [LibraryImport(LIB, EntryPoint = "glGetUniformuiv")]
        public static partial void glGetUniformuiv(uint program, int location, uint* @params);

        [LibraryImport(LIB, EntryPoint = "glGetFragDataLocation", StringMarshalling = StringMarshalling.Utf8)]
        public static partial int glGetFragDataLocation(uint program, string name);

        [LibraryImport(LIB, EntryPoint = "glUniform1ui")]
        public static partial void glUniform1ui(int location, uint v0);

        [LibraryImport(LIB, EntryPoint = "glUniform2ui")]
        public static partial void glUniform2ui(int location, uint v0, uint v1);

        [LibraryImport(LIB, EntryPoint = "glUniform3ui")]
        public static partial void glUniform3ui(int location, uint v0, uint v1, uint v2);

        [LibraryImport(LIB, EntryPoint = "glUniform4ui")]
        public static partial void glUniform4ui(int location, uint v0, uint v1, uint v2, uint v3);

        [LibraryImport(LIB, EntryPoint = "glUniform1uiv")]
        public static partial void glUniform1uiv(int location, int count, uint* value);

        [LibraryImport(LIB, EntryPoint = "glUniform2uiv")]
        public static partial void glUniform2uiv(int location, int count, uint* value);

        [LibraryImport(LIB, EntryPoint = "glUniform3uiv")]
        public static partial void glUniform3uiv(int location, int count, uint* value);

        [LibraryImport(LIB, EntryPoint = "glUniform4uiv")]
        public static partial void glUniform4uiv(int location, int count, uint* value);

        [LibraryImport(LIB, EntryPoint = "glClearBufferiv")]
        public static partial void glClearBufferiv(int buffer, int drawbuffer, int* value);

        [LibraryImport(LIB, EntryPoint = "glClearBufferuiv")]
        public static partial void glClearBufferuiv(int buffer, int drawbuffer, uint* value);

        [LibraryImport(LIB, EntryPoint = "glClearBufferfv")]
        public static partial void glClearBufferfv(int buffer, int drawbuffer, float* value);

        [LibraryImport(LIB, EntryPoint = "glClearBufferfi")]
        public static partial void glClearBufferfi(int buffer, int drawbuffer,
            float depth, int stencil);

        [LibraryImport(LIB, EntryPoint = "glGetStringi")]
        private static partial IntPtr _glGetStringi(int name, uint index);

        public static string glGetStringi(int name, uint index)
            => PtrToStringUtf8(_glGetStringi(name, index));

        [LibraryImport(LIB, EntryPoint = "glCopyBufferSubData")]
        public static partial void glCopyBufferSubData(int readTarget, int writeTarget,
            IntPtr readOffset, IntPtr writeOffset, IntPtr size);

        [LibraryImport(LIB, EntryPoint = "glGetUniformIndices")]
        public static partial void glGetUniformIndices(uint program, int uniformCount,
            byte** uniformNames, uint* uniformIndices);

        [LibraryImport(LIB, EntryPoint = "glGetActiveUniformsiv")]
        public static partial void glGetActiveUniformsiv(uint program, int uniformCount,
            uint* uniformIndices, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetUniformBlockIndex",
            StringMarshalling = StringMarshalling.Utf8)]
        public static partial uint glGetUniformBlockIndex(uint program, string uniformBlockName);

        [LibraryImport(LIB, EntryPoint = "glGetActiveUniformBlockiv")]
        public static partial void glGetActiveUniformBlockiv(uint program,
            uint uniformBlockIndex, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetActiveUniformBlockName")]
        public static partial void glGetActiveUniformBlockName(uint program,
            uint uniformBlockIndex, int bufSize, int* length, byte* uniformBlockName);

        [LibraryImport(LIB, EntryPoint = "glUniformBlockBinding")]
        public static partial void glUniformBlockBinding(uint program,
            uint uniformBlockIndex, uint uniformBlockBinding);

        [LibraryImport(LIB, EntryPoint = "glFenceSync")]
        public static partial IntPtr glFenceSync(int condition, uint flags);

        [LibraryImport(LIB, EntryPoint = "glIsSync")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsSync(IntPtr sync);

        [LibraryImport(LIB, EntryPoint = "glDeleteSync")]
        public static partial void glDeleteSync(IntPtr sync);

        [LibraryImport(LIB, EntryPoint = "glClientWaitSync")]
        public static partial int glClientWaitSync(IntPtr sync, uint flags, ulong timeout);

        [LibraryImport(LIB, EntryPoint = "glWaitSync")]
        public static partial void glWaitSync(IntPtr sync, uint flags, ulong timeout);

        [LibraryImport(LIB, EntryPoint = "glGetInteger64v")]
        public static partial void glGetInteger64v(int pname, long* data);

        [LibraryImport(LIB, EntryPoint = "glGetSynciv")]
        public static partial void glGetSynciv(IntPtr sync, int pname, int bufSize,
            int* length, int* values);

        [LibraryImport(LIB, EntryPoint = "glGetInteger64i_v")]
        public static partial void glGetInteger64i_v(int target, uint index, long* data);

        [LibraryImport(LIB, EntryPoint = "glGetBufferParameteri64v")]
        public static partial void glGetBufferParameteri64v(int target, int pname, long* @params);

        [LibraryImport(LIB, EntryPoint = "glGenSamplers")]
        public static partial void glGenSamplers(int count, uint* samplers);

        public static uint glGenSampler()
        {
            uint id = 0;
            glGenSamplers(1, &id);
            return id;
        }

        [LibraryImport(LIB, EntryPoint = "glDeleteSamplers")]
        public static partial void glDeleteSamplers(int count, uint* samplers);

        public static void glDeleteSampler(uint sampler)
            => glDeleteSamplers(1, &sampler);

        [LibraryImport(LIB, EntryPoint = "glIsSampler")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsSampler(uint sampler);

        [LibraryImport(LIB, EntryPoint = "glBindSampler")]
        public static partial void glBindSampler(uint unit, uint sampler);

        [LibraryImport(LIB, EntryPoint = "glSamplerParameteri")]
        public static partial void glSamplerParameteri(uint sampler, int pname, int param);

        [LibraryImport(LIB, EntryPoint = "glSamplerParameteriv")]
        public static partial void glSamplerParameteriv(uint sampler, int pname, int* param);

        [LibraryImport(LIB, EntryPoint = "glSamplerParameterf")]
        public static partial void glSamplerParameterf(uint sampler, int pname, float param);

        [LibraryImport(LIB, EntryPoint = "glSamplerParameterfv")]
        public static partial void glSamplerParameterfv(uint sampler, int pname, float* param);

        [LibraryImport(LIB, EntryPoint = "glGetSamplerParameteriv")]
        public static partial void glGetSamplerParameteriv(uint sampler, int pname, int* @params);

        [LibraryImport(LIB, EntryPoint = "glGetSamplerParameterfv")]
        public static partial void glGetSamplerParameterfv(uint sampler, int pname, float* @params);

        [LibraryImport(LIB, EntryPoint = "glVertexAttribDivisor")]
        public static partial void glVertexAttribDivisor(uint index, uint divisor);

        [LibraryImport(LIB, EntryPoint = "glBindTransformFeedback")]
        public static partial void glBindTransformFeedback(int target, uint id);

        [LibraryImport(LIB, EntryPoint = "glDeleteTransformFeedbacks")]
        public static partial void glDeleteTransformFeedbacks(int n, uint* ids);

        public static void glDeleteTransformFeedback(uint id)
            => glDeleteTransformFeedbacks(1, &id);

        [LibraryImport(LIB, EntryPoint = "glGenTransformFeedbacks")]
        public static partial void glGenTransformFeedbacks(int n, uint* ids);

        public static uint glGenTransformFeedback()
        {
            uint id = 0;
            glGenTransformFeedbacks(1, &id);
            return id;
        }

        [LibraryImport(LIB, EntryPoint = "glIsTransformFeedback")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static partial bool glIsTransformFeedback(uint id);

        [LibraryImport(LIB, EntryPoint = "glPauseTransformFeedback")]
        public static partial void glPauseTransformFeedback();

        [LibraryImport(LIB, EntryPoint = "glResumeTransformFeedback")]
        public static partial void glResumeTransformFeedback();

        [LibraryImport(LIB, EntryPoint = "glGetProgramBinary")]
        public static partial void glGetProgramBinary(uint program, int bufSize,
            int* length, int* binaryFormat, void* binary);

        [LibraryImport(LIB, EntryPoint = "glProgramBinary")]
        public static partial void glProgramBinary(uint program, int binaryFormat,
            void* binary, int length);

        [LibraryImport(LIB, EntryPoint = "glProgramParameteri")]
        public static partial void glProgramParameteri(uint program, int pname, int value);

        [LibraryImport(LIB, EntryPoint = "glInvalidateFramebuffer")]
        public static partial void glInvalidateFramebuffer(int target,
            int numAttachments, int* attachments);

        [LibraryImport(LIB, EntryPoint = "glInvalidateSubFramebuffer")]
        public static partial void glInvalidateSubFramebuffer(int target,
            int numAttachments, int* attachments, int x, int y, int width, int height);

        [LibraryImport(LIB, EntryPoint = "glTexStorage2D")]
        public static partial void glTexStorage2D(int target, int levels,
            int internalformat, int width, int height);

        [LibraryImport(LIB, EntryPoint = "glTexStorage3D")]
        public static partial void glTexStorage3D(int target, int levels,
            int internalformat, int width, int height, int depth);

        [LibraryImport(LIB, EntryPoint = "glGetInternalformativ")]
        public static partial void glGetInternalformativ(int target, int internalformat,
            int pname, int bufSize, int* @params);

        [LibraryImport(LIB, EntryPoint = "glDrawArraysInstanced")]
        public static partial void glDrawArraysInstanced(int mode, int first,
            int count, int instancecount);

        [LibraryImport(LIB, EntryPoint = "glDrawElementsInstanced")]
        public static partial void glDrawElementsInstanced(int mode, int count,
            int type, void* indices, int instancecount);

        // ── Constants (GLES 3.0 only) ─────────────────────────────────────────

        #region Constants

        // Buffers / clear
        public const uint GL_DEPTH_BUFFER_BIT = 0x00000100u;
        public const uint GL_STENCIL_BUFFER_BIT = 0x00000400u;
        public const uint GL_COLOR_BUFFER_BIT = 0x00004000u;

        public const int GL_FALSE = 0;
        public const int GL_TRUE = 1;

        // Primitives
        public const int GL_POINTS = 0x0000;
        public const int GL_LINES = 0x0001;
        public const int GL_LINE_LOOP = 0x0002;
        public const int GL_LINE_STRIP = 0x0003;
        public const int GL_TRIANGLES = 0x0004;
        public const int GL_TRIANGLE_STRIP = 0x0005;
        public const int GL_TRIANGLE_FAN = 0x0006;

        // Depth / stencil functions
        public const int GL_NEVER = 0x0200;
        public const int GL_LESS = 0x0201;
        public const int GL_EQUAL = 0x0202;
        public const int GL_LEQUAL = 0x0203;
        public const int GL_GREATER = 0x0204;
        public const int GL_NOTEQUAL = 0x0205;
        public const int GL_GEQUAL = 0x0206;
        public const int GL_ALWAYS = 0x0207;

        // Blend factors
        public const int GL_ZERO = 0;
        public const int GL_ONE = 1;
        public const int GL_SRC_COLOR = 0x0300;
        public const int GL_ONE_MINUS_SRC_COLOR = 0x0301;
        public const int GL_SRC_ALPHA = 0x0302;
        public const int GL_ONE_MINUS_SRC_ALPHA = 0x0303;
        public const int GL_DST_ALPHA = 0x0304;
        public const int GL_ONE_MINUS_DST_ALPHA = 0x0305;
        public const int GL_DST_COLOR = 0x0306;
        public const int GL_ONE_MINUS_DST_COLOR = 0x0307;
        public const int GL_SRC_ALPHA_SATURATE = 0x0308;
        public const int GL_CONSTANT_COLOR = 0x8001;
        public const int GL_ONE_MINUS_CONSTANT_COLOR = 0x8002;
        public const int GL_CONSTANT_ALPHA = 0x8003;
        public const int GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004;
        public const int GL_SRC1_ALPHA = 0x8589;
        public const int GL_SRC1_COLOR = 0x88F9;
        public const int GL_ONE_MINUS_SRC1_COLOR = 0x88FA;
        public const int GL_ONE_MINUS_SRC1_ALPHA = 0x88FB;

        // Draw buffers
        public const int GL_NONE = 0;
        public const int GL_FRONT = 0x0404;
        public const int GL_BACK = 0x0405;
        public const int GL_FRONT_AND_BACK = 0x0408;

        // Errors
        public const int GL_NO_ERROR = 0;
        public const int GL_INVALID_ENUM = 0x0500;
        public const int GL_INVALID_VALUE = 0x0501;
        public const int GL_INVALID_OPERATION = 0x0502;
        public const int GL_OUT_OF_MEMORY = 0x0505;
        public const int GL_INVALID_FRAMEBUFFER_OPERATION = 0x0506;

        // Winding
        public const int GL_CW = 0x0900;
        public const int GL_CCW = 0x0901;

        // Capabilities
        public const int GL_CULL_FACE = 0x0B44;
        public const int GL_CULL_FACE_MODE = 0x0B45;
        public const int GL_FRONT_FACE = 0x0B46;
        public const int GL_DEPTH_RANGE = 0x0B70;
        public const int GL_DEPTH_TEST = 0x0B71;
        public const int GL_DEPTH_WRITEMASK = 0x0B72;
        public const int GL_DEPTH_CLEAR_VALUE = 0x0B73;
        public const int GL_DEPTH_FUNC = 0x0B74;
        public const int GL_STENCIL_TEST = 0x0B90;
        public const int GL_STENCIL_CLEAR_VALUE = 0x0B91;
        public const int GL_STENCIL_FUNC = 0x0B92;
        public const int GL_STENCIL_VALUE_MASK = 0x0B93;
        public const int GL_STENCIL_FAIL = 0x0B94;
        public const int GL_STENCIL_PASS_DEPTH_FAIL = 0x0B95;
        public const int GL_STENCIL_PASS_DEPTH_PASS = 0x0B96;
        public const int GL_STENCIL_REF = 0x0B97;
        public const int GL_STENCIL_WRITEMASK = 0x0B98;
        public const int GL_VIEWPORT = 0x0BA2;
        public const int GL_DITHER = 0x0BD0;
        public const int GL_BLEND = 0x0BE2;
        public const int GL_SCISSOR_BOX = 0x0C10;
        public const int GL_SCISSOR_TEST = 0x0C11;
        public const int GL_COLOR_CLEAR_VALUE = 0x0C22;
        public const int GL_COLOR_WRITEMASK = 0x0C23;
        public const int GL_LINE_WIDTH = 0x0B21;
        public const int GL_ALIASED_LINE_WIDTH_RANGE = 0x846E;
        public const int GL_ALIASED_POINT_SIZE_RANGE = 0x846D;

        // Pixel store
        public const int GL_UNPACK_ROW_LENGTH = 0x0CF2;
        public const int GL_UNPACK_SKIP_ROWS = 0x0CF3;
        public const int GL_UNPACK_SKIP_PIXELS = 0x0CF4;
        public const int GL_UNPACK_ALIGNMENT = 0x0CF5;
        public const int GL_PACK_ROW_LENGTH = 0x0D02;
        public const int GL_PACK_SKIP_ROWS = 0x0D03;
        public const int GL_PACK_SKIP_PIXELS = 0x0D04;
        public const int GL_PACK_ALIGNMENT = 0x0D05;
        public const int GL_UNPACK_SKIP_IMAGES = 0x806D;
        public const int GL_UNPACK_IMAGE_HEIGHT = 0x806E;

        // Textures
        public const int GL_MAX_TEXTURE_SIZE = 0x0D33;
        public const int GL_MAX_VIEWPORT_DIMS = 0x0D3A;
        public const int GL_SUBPIXEL_BITS = 0x0D50;
        public const int GL_TEXTURE_2D = 0x0DE1;
        public const int GL_TEXTURE_3D = 0x806F;
        public const int GL_TEXTURE_2D_ARRAY = 0x8C1A;
        public const int GL_TEXTURE_CUBE_MAP = 0x8513;
        public const int GL_TEXTURE_BINDING_2D = 0x8069;
        public const int GL_TEXTURE_BINDING_3D = 0x806A;
        public const int GL_TEXTURE_BINDING_2D_ARRAY = 0x8C1D;
        public const int GL_TEXTURE_BINDING_CUBE_MAP = 0x8514;
        public const int GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515;
        public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_X = 0x8516;
        public const int GL_TEXTURE_CUBE_MAP_POSITIVE_Y = 0x8517;
        public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_Y = 0x8518;
        public const int GL_TEXTURE_CUBE_MAP_POSITIVE_Z = 0x8519;
        public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_Z = 0x851A;
        public const int GL_MAX_CUBE_MAP_TEXTURE_SIZE = 0x851C;
        public const int GL_TEXTURE_WIDTH = 0x1000;
        public const int GL_TEXTURE_HEIGHT = 0x1001;
        public const int GL_TEXTURE_INTERNAL_FORMAT = 0x1003;
        public const int GL_TEXTURE_DEPTH = 0x8071;
        public const int GL_TEXTURE_WRAP_R = 0x8072;
        public const int GL_MAX_3D_TEXTURE_SIZE = 0x8073;
        public const int GL_MAX_ARRAY_TEXTURE_LAYERS = 0x88FF;
        public const int GL_DONT_CARE = 0x1100;
        public const int GL_FASTEST = 0x1101;
        public const int GL_NICEST = 0x1102;

        // Data types
        public const int GL_BYTE = 0x1400;
        public const int GL_UNSIGNED_BYTE = 0x1401;
        public const int GL_SHORT = 0x1402;
        public const int GL_UNSIGNED_SHORT = 0x1403;
        public const int GL_INT = 0x1404;
        public const int GL_UNSIGNED_INT = 0x1405;
        public const int GL_FLOAT = 0x1406;
        public const int GL_HALF_FLOAT = 0x140B;
        public const int GL_UNSIGNED_SHORT_4_4_4_4 = 0x8033;
        public const int GL_UNSIGNED_SHORT_5_5_5_1 = 0x8034;
        public const int GL_UNSIGNED_SHORT_5_6_5 = 0x8363;
        public const int GL_UNSIGNED_INT_2_10_10_10_REV = 0x8368;
        public const int GL_UNSIGNED_INT_10F_11F_11F_REV = 0x8C3B;
        public const int GL_UNSIGNED_INT_5_9_9_9_REV = 0x8C3E;
        public const int GL_UNSIGNED_INT_24_8 = 0x84FA;
        public const int GL_FLOAT_32_UNSIGNED_INT_24_8_REV = 0x8DAD;
        public const int GL_INT_2_10_10_10_REV = 0x8D9F;

        // Pixel formats
        public const int GL_DEPTH_COMPONENT = 0x1902;
        public const int GL_RED = 0x1903;
        public const int GL_RGB = 0x1907;
        public const int GL_RGBA = 0x1908;
        public const int GL_STENCIL_INDEX = 0x1901;
        public const int GL_DEPTH_STENCIL = 0x84F9;
        public const int GL_RG = 0x8227;
        public const int GL_RG_INTEGER = 0x8228;
        public const int GL_RED_INTEGER = 0x8D94;
        public const int GL_RGB_INTEGER = 0x8D98;
        public const int GL_RGBA_INTEGER = 0x8D99;
        public const int GL_BGRA = 0x80E1;
        // Stencil ops
        public const int GL_KEEP = 0x1E00;
        public const int GL_REPLACE = 0x1E01;
        public const int GL_INCR = 0x1E02;
        public const int GL_DECR = 0x1E03;
        public const int GL_INCR_WRAP = 0x8507;
        public const int GL_DECR_WRAP = 0x8508;
        public const int GL_INVERT = 0x150A;

        // String queries
        public const int GL_VENDOR = 0x1F00;
        public const int GL_RENDERER = 0x1F01;
        public const int GL_VERSION = 0x1F02;
        public const int GL_EXTENSIONS = 0x1F03;
        public const int GL_SHADING_LANGUAGE_VERSION = 0x8B8C;
        public const int GL_NUM_EXTENSIONS = 0x821D;

        // Texture filters / wrap
        public const int GL_NEAREST = 0x2600;
        public const int GL_LINEAR = 0x2601;
        public const int GL_NEAREST_MIPMAP_NEAREST = 0x2700;
        public const int GL_LINEAR_MIPMAP_NEAREST = 0x2701;
        public const int GL_NEAREST_MIPMAP_LINEAR = 0x2702;
        public const int GL_LINEAR_MIPMAP_LINEAR = 0x2703;
        public const int GL_TEXTURE_MAG_FILTER = 0x2800;
        public const int GL_TEXTURE_MIN_FILTER = 0x2801;
        public const int GL_TEXTURE_WRAP_S = 0x2802;
        public const int GL_TEXTURE_WRAP_T = 0x2803;
        public const int GL_REPEAT = 0x2901;
        public const int GL_CLAMP_TO_EDGE = 0x812F;
        public const int GL_MIRRORED_REPEAT = 0x8370;
        public const int GL_TEXTURE_MIN_LOD = 0x813A;
        public const int GL_TEXTURE_MAX_LOD = 0x813B;
        public const int GL_TEXTURE_BASE_LEVEL = 0x813C;
        public const int GL_TEXTURE_MAX_LEVEL = 0x813D;
        public const int GL_TEXTURE_COMPARE_MODE = 0x884C;
        public const int GL_TEXTURE_COMPARE_FUNC = 0x884D;
        public const int GL_COMPARE_REF_TO_TEXTURE = 0x884E;
        public const int GL_TEXTURE_SWIZZLE_R = 0x8E42;
        public const int GL_TEXTURE_SWIZZLE_G = 0x8E43;
        public const int GL_TEXTURE_SWIZZLE_B = 0x8E44;
        public const int GL_TEXTURE_SWIZZLE_A = 0x8E45;
        public const int GL_TEXTURE_SWIZZLE_RGBA = 0x8E46;

        // Polygon offset
        public const int GL_POLYGON_OFFSET_FILL = 0x8037;
        public const int GL_POLYGON_OFFSET_FACTOR = 0x8038;
        public const int GL_POLYGON_OFFSET_UNITS = 0x2A00;

        // Internal texture formats (GLES 3.0 sized)
        public const int GL_RGBA4 = 0x8056;
        public const int GL_RGB5_A1 = 0x8057;
        public const int GL_RGBA8 = 0x8058;
        public const int GL_RGB10_A2 = 0x8059;
        public const int GL_RGB8 = 0x8051;
        public const int GL_RGB565 = 0x8D62;
        public const int GL_SRGB8 = 0x8C41;
        public const int GL_SRGB8_ALPHA8 = 0x8C43;
        public const int GL_R8 = 0x8229;
        public const int GL_RG8 = 0x822B;
        public const int GL_R16F = 0x822D;
        public const int GL_R32F = 0x822E;
        public const int GL_RG16F = 0x822F;
        public const int GL_RG32F = 0x8230;
        public const int GL_R8I = 0x8231;
        public const int GL_R8UI = 0x8232;
        public const int GL_R16I = 0x8233;
        public const int GL_R16UI = 0x8234;
        public const int GL_R32I = 0x8235;
        public const int GL_R32UI = 0x8236;
        public const int GL_RG8I = 0x8237;
        public const int GL_RG8UI = 0x8238;
        public const int GL_RG16I = 0x8239;
        public const int GL_RG16UI = 0x823A;
        public const int GL_RG32I = 0x823B;
        public const int GL_RG32UI = 0x823C;
        public const int GL_RGBA32F = 0x8814;
        public const int GL_RGB32F = 0x8815;
        public const int GL_RGBA16F = 0x881A;
        public const int GL_RGB16F = 0x881B;
        public const int GL_RGBA32UI = 0x8D70;
        public const int GL_RGB32UI = 0x8D71;
        public const int GL_RGBA16UI = 0x8D76;
        public const int GL_RGB16UI = 0x8D77;
        public const int GL_RGBA8UI = 0x8D7C;
        public const int GL_RGB8UI = 0x8D7D;
        public const int GL_RGBA32I = 0x8D82;
        public const int GL_RGB32I = 0x8D83;
        public const int GL_RGBA16I = 0x8D88;
        public const int GL_RGB16I = 0x8D89;
        public const int GL_RGBA8I = 0x8D8E;
        public const int GL_RGB8I = 0x8D8F;
        public const int GL_R8_SNORM = 0x8F94;
        public const int GL_RG8_SNORM = 0x8F95;
        public const int GL_RGB8_SNORM = 0x8F96;
        public const int GL_RGBA8_SNORM = 0x8F97;
        public const int GL_RGB10_A2UI = 0x906F;
        public const int GL_R11F_G11F_B10F = 0x8C3A;
        public const int GL_RGB9_E5 = 0x8C3D;

        // Depth / stencil internal formats
        public const int GL_DEPTH_COMPONENT16 = 0x81A5;
        public const int GL_DEPTH_COMPONENT24 = 0x81A6;
        public const int GL_DEPTH_COMPONENT32F = 0x8CAC;
        public const int GL_DEPTH24_STENCIL8 = 0x88F0;
        public const int GL_DEPTH32F_STENCIL8 = 0x8CAD;
        public const int GL_STENCIL_INDEX8 = 0x8D48;

        // Compressed texture formats (ETC2 — mandatory in GLES 3.0)
        public const int GL_COMPRESSED_R11_EAC = 0x9270;
        public const int GL_COMPRESSED_SIGNED_R11_EAC = 0x9271;
        public const int GL_COMPRESSED_RG11_EAC = 0x9272;
        public const int GL_COMPRESSED_SIGNED_RG11_EAC = 0x9273;
        public const int GL_COMPRESSED_RGB8_ETC2 = 0x9274;
        public const int GL_COMPRESSED_SRGB8_ETC2 = 0x9275;
        public const int GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9276;
        public const int GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9277;
        public const int GL_COMPRESSED_RGBA8_ETC2_EAC = 0x9278;
        public const int GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 0x9279;

        // Multisample
        public const int GL_MULTISAMPLE = 0x809D;
        public const int GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E;
        public const int GL_SAMPLE_COVERAGE = 0x80A0;
        public const int GL_SAMPLE_BUFFERS = 0x80A8;
        public const int GL_SAMPLES = 0x80A9;
        public const int GL_SAMPLE_COVERAGE_VALUE = 0x80AA;
        public const int GL_SAMPLE_COVERAGE_INVERT = 0x80AB;
        public const int GL_MAX_SAMPLES = 0x8D57;

        // Blend equations
        public const int GL_BLEND_COLOR = 0x8005;
        public const int GL_BLEND_EQUATION = 0x8009;
        public const int GL_BLEND_EQUATION_RGB = 0x8009;
        public const int GL_BLEND_EQUATION_ALPHA = 0x883D;
        public const int GL_FUNC_ADD = 0x8006;
        public const int GL_FUNC_SUBTRACT = 0x800A;
        public const int GL_FUNC_REVERSE_SUBTRACT = 0x800B;
        public const int GL_MIN = 0x8007;
        public const int GL_MAX = 0x8008;
        public const int GL_BLEND_DST_RGB = 0x80C8;
        public const int GL_BLEND_SRC_RGB = 0x80C9;
        public const int GL_BLEND_DST_ALPHA = 0x80CA;
        public const int GL_BLEND_SRC_ALPHA = 0x80CB;

        // Buffer objects
        public const int GL_BUFFER_SIZE = 0x8764;
        public const int GL_BUFFER_USAGE = 0x8765;
        public const int GL_ARRAY_BUFFER = 0x8892;
        public const int GL_ELEMENT_ARRAY_BUFFER = 0x8893;
        public const int GL_ARRAY_BUFFER_BINDING = 0x8894;
        public const int GL_ELEMENT_ARRAY_BUFFER_BINDING = 0x8895;
        public const int GL_PIXEL_PACK_BUFFER = 0x88EB;
        public const int GL_PIXEL_UNPACK_BUFFER = 0x88EC;
        public const int GL_PIXEL_PACK_BUFFER_BINDING = 0x88ED;
        public const int GL_PIXEL_UNPACK_BUFFER_BINDING = 0x88EF;
        public const int GL_COPY_READ_BUFFER = 0x8F36;
        public const int GL_COPY_WRITE_BUFFER = 0x8F37;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER = 0x8C8E;
        public const int GL_UNIFORM_BUFFER = 0x8A11;
        public const int GL_STREAM_DRAW = 0x88E0;
        public const int GL_STREAM_READ = 0x88E1;
        public const int GL_STREAM_COPY = 0x88E2;
        public const int GL_STATIC_DRAW = 0x88E4;
        public const int GL_STATIC_READ = 0x88E5;
        public const int GL_STATIC_COPY = 0x88E6;
        public const int GL_DYNAMIC_DRAW = 0x88E8;
        public const int GL_DYNAMIC_READ = 0x88E9;
        public const int GL_DYNAMIC_COPY = 0x88EA;
        public const int GL_BUFFER_ACCESS_FLAGS = 0x911F;
        public const int GL_BUFFER_MAP_LENGTH = 0x9120;
        public const int GL_BUFFER_MAP_OFFSET = 0x9121;
        public const int GL_BUFFER_MAPPED = 0x88BC;
        public const int GL_BUFFER_MAP_POINTER = 0x88BD;

        // Map buffer range
        public const int GL_MAP_READ_BIT = 0x0001;
        public const int GL_MAP_WRITE_BIT = 0x0002;
        public const int GL_MAP_INVALIDATE_RANGE_BIT = 0x0004;
        public const int GL_MAP_INVALIDATE_BUFFER_BIT = 0x0008;
        public const int GL_MAP_FLUSH_EXPLICIT_BIT = 0x0010;
        public const int GL_MAP_UNSYNCHRONIZED_BIT = 0x0020;

        // Query objects
        public const int GL_QUERY_RESULT = 0x8866;
        public const int GL_QUERY_RESULT_AVAILABLE = 0x8867;
        public const int GL_ANY_SAMPLES_PASSED = 0x8C2F;
        public const int GL_ANY_SAMPLES_PASSED_CONSERVATIVE = 0x8D6A;
        public const int GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 0x8C88;

        // Active texture units
        public const int GL_TEXTURE0 = 0x84C0;
        public const int GL_TEXTURE1 = 0x84C1;
        public const int GL_TEXTURE2 = 0x84C2;
        public const int GL_TEXTURE3 = 0x84C3;
        public const int GL_TEXTURE4 = 0x84C4;
        public const int GL_TEXTURE5 = 0x84C5;
        public const int GL_TEXTURE6 = 0x84C6;
        public const int GL_TEXTURE7 = 0x84C7;
        public const int GL_TEXTURE8 = 0x84C8;
        public const int GL_TEXTURE9 = 0x84C9;
        public const int GL_TEXTURE10 = 0x84CA;
        public const int GL_TEXTURE11 = 0x84CB;
        public const int GL_TEXTURE12 = 0x84CC;
        public const int GL_TEXTURE13 = 0x84CD;
        public const int GL_TEXTURE14 = 0x84CE;
        public const int GL_TEXTURE15 = 0x84CF;
        public const int GL_TEXTURE16 = 0x84D0;
        public const int GL_TEXTURE17 = 0x84D1;
        public const int GL_TEXTURE18 = 0x84D2;
        public const int GL_TEXTURE19 = 0x84D3;
        public const int GL_TEXTURE20 = 0x84D4;
        public const int GL_TEXTURE21 = 0x84D5;
        public const int GL_TEXTURE22 = 0x84D6;
        public const int GL_TEXTURE23 = 0x84D7;
        public const int GL_TEXTURE24 = 0x84D8;
        public const int GL_TEXTURE25 = 0x84D9;
        public const int GL_TEXTURE26 = 0x84DA;
        public const int GL_TEXTURE27 = 0x84DB;
        public const int GL_TEXTURE28 = 0x84DC;
        public const int GL_TEXTURE29 = 0x84DD;
        public const int GL_TEXTURE30 = 0x84DE;
        public const int GL_TEXTURE31 = 0x84DF;
        public const int GL_ACTIVE_TEXTURE = 0x84E0;
        public const int GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872;
        public const int GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C;
        public const int GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D;

        // Shader / program
        public const int GL_FRAGMENT_SHADER = 0x8B30;
        public const int GL_VERTEX_SHADER = 0x8B31;
        public const int GL_DELETE_STATUS = 0x8B80;
        public const int GL_COMPILE_STATUS = 0x8B81;
        public const int GL_LINK_STATUS = 0x8B82;
        public const int GL_VALIDATE_STATUS = 0x8B83;
        public const int GL_INFO_LOG_LENGTH = 0x8B84;
        public const int GL_ATTACHED_SHADERS = 0x8B85;
        public const int GL_ACTIVE_UNIFORMS = 0x8B86;
        public const int GL_ACTIVE_ATTRIBUTES = 0x8B89;
        public const int GL_SHADER_TYPE = 0x8B4F;
        public const int GL_CURRENT_PROGRAM = 0x8B8D;
        public const int GL_SHADER_SOURCE_LENGTH = 0x8B88;
        public const int GL_ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87;
        public const int GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A;
        public const int GL_MAX_VERTEX_ATTRIBS = 0x8869;
        public const int GL_MAX_VERTEX_UNIFORM_VECTORS = 0x8DFB;
        public const int GL_MAX_VARYING_VECTORS = 0x8DFC;
        public const int GL_MAX_FRAGMENT_UNIFORM_VECTORS = 0x8DFD;
        public const int GL_MAX_VERTEX_UNIFORM_COMPONENTS = 0x8B4A;
        public const int GL_MAX_FRAGMENT_UNIFORM_COMPONENTS = 0x8B49;
        public const int GL_MAX_VERTEX_OUTPUT_COMPONENTS = 0x9122;
        public const int GL_MAX_FRAGMENT_INPUT_COMPONENTS = 0x9125;
        public const int GL_FRAGMENT_SHADER_DERIVATIVE_HINT = 0x8B8B;
        public const int GL_PROGRAM_BINARY_RETRIEVABLE_HINT = 0x8257;
        public const int GL_PROGRAM_BINARY_LENGTH = 0x8741;
        public const int GL_NUM_PROGRAM_BINARY_FORMATS = 0x87FE;
        public const int GL_PROGRAM_BINARY_FORMATS = 0x87FF;

        // Uniform types
        public const int GL_FLOAT_VEC2 = 0x8B50;
        public const int GL_FLOAT_VEC3 = 0x8B51;
        public const int GL_FLOAT_VEC4 = 0x8B52;
        public const int GL_INT_VEC2 = 0x8B53;
        public const int GL_INT_VEC3 = 0x8B54;
        public const int GL_INT_VEC4 = 0x8B55;
        public const int GL_BOOL = 0x8B56;
        public const int GL_BOOL_VEC2 = 0x8B57;
        public const int GL_BOOL_VEC3 = 0x8B58;
        public const int GL_BOOL_VEC4 = 0x8B59;
        public const int GL_FLOAT_MAT2 = 0x8B5A;
        public const int GL_FLOAT_MAT3 = 0x8B5B;
        public const int GL_FLOAT_MAT4 = 0x8B5C;
        public const int GL_FLOAT_MAT2x3 = 0x8B65;
        public const int GL_FLOAT_MAT2x4 = 0x8B66;
        public const int GL_FLOAT_MAT3x2 = 0x8B67;
        public const int GL_FLOAT_MAT3x4 = 0x8B68;
        public const int GL_FLOAT_MAT4x2 = 0x8B69;
        public const int GL_FLOAT_MAT4x3 = 0x8B6A;
        public const int GL_SAMPLER_2D = 0x8B5E;
        public const int GL_SAMPLER_3D = 0x8B5F;
        public const int GL_SAMPLER_CUBE = 0x8B60;
        public const int GL_SAMPLER_2D_SHADOW = 0x8B62;
        public const int GL_SAMPLER_2D_ARRAY = 0x8DC1;
        public const int GL_SAMPLER_2D_ARRAY_SHADOW = 0x8DC4;
        public const int GL_SAMPLER_CUBE_SHADOW = 0x8DC5;
        public const int GL_UNSIGNED_INT_VEC2 = 0x8DC6;
        public const int GL_UNSIGNED_INT_VEC3 = 0x8DC7;
        public const int GL_UNSIGNED_INT_VEC4 = 0x8DC8;
        public const int GL_INT_SAMPLER_2D = 0x8DCA;
        public const int GL_INT_SAMPLER_3D = 0x8DCB;
        public const int GL_INT_SAMPLER_CUBE = 0x8DCC;
        public const int GL_INT_SAMPLER_2D_ARRAY = 0x8DCF;
        public const int GL_UNSIGNED_INT_SAMPLER_2D = 0x8DD2;
        public const int GL_UNSIGNED_INT_SAMPLER_3D = 0x8DD3;
        public const int GL_UNSIGNED_INT_SAMPLER_CUBE = 0x8DD4;
        public const int GL_UNSIGNED_INT_SAMPLER_2D_ARRAY = 0x8DD7;

        // Vertex attribs
        public const int GL_VERTEX_ATTRIB_ARRAY_ENABLED = 0x8622;
        public const int GL_VERTEX_ATTRIB_ARRAY_SIZE = 0x8623;
        public const int GL_VERTEX_ATTRIB_ARRAY_STRIDE = 0x8624;
        public const int GL_VERTEX_ATTRIB_ARRAY_TYPE = 0x8625;
        public const int GL_CURRENT_VERTEX_ATTRIB = 0x8626;
        public const int GL_VERTEX_ATTRIB_ARRAY_NORMALIZED = 0x886A;
        public const int GL_VERTEX_ATTRIB_ARRAY_POINTER = 0x8645;
        public const int GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 0x889F;
        public const int GL_VERTEX_ATTRIB_ARRAY_INTEGER = 0x88FD;
        public const int GL_VERTEX_ATTRIB_ARRAY_DIVISOR = 0x88FE;
        public const int GL_VERTEX_ARRAY_BINDING = 0x85B5;

        // Framebuffer / renderbuffer
        public const int GL_FRAMEBUFFER = 0x8D40;
        public const int GL_RENDERBUFFER = 0x8D41;
        public const int GL_FRAMEBUFFER_COMPLETE = 0x8CD5;
        public const int GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 0x8CD6;
        public const int GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7;
        public const int GL_FRAMEBUFFER_UNSUPPORTED = 0x8CDD;
        public const int GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE = 0x8D56;
        public const int GL_FRAMEBUFFER_UNDEFINED = 0x8219;
        public const int GL_FRAMEBUFFER_BINDING = 0x8CA6;
        public const int GL_DRAW_FRAMEBUFFER_BINDING = 0x8CA6;
        public const int GL_READ_FRAMEBUFFER = 0x8CA8;
        public const int GL_DRAW_FRAMEBUFFER = 0x8CA9;
        public const int GL_READ_FRAMEBUFFER_BINDING = 0x8CAA;
        public const int GL_RENDERBUFFER_BINDING = 0x8CA7;
        public const int GL_RENDERBUFFER_SAMPLES = 0x8CAB;
        public const int GL_MAX_RENDERBUFFER_SIZE = 0x84E8;
        public const int GL_MAX_COLOR_ATTACHMENTS = 0x8CDF;
        public const int GL_MAX_DRAW_BUFFERS = 0x8824;
        public const int GL_COLOR_ATTACHMENT0 = 0x8CE0;
        public const int GL_COLOR_ATTACHMENT1 = 0x8CE1;
        public const int GL_COLOR_ATTACHMENT2 = 0x8CE2;
        public const int GL_COLOR_ATTACHMENT3 = 0x8CE3;
        public const int GL_COLOR_ATTACHMENT4 = 0x8CE4;
        public const int GL_COLOR_ATTACHMENT5 = 0x8CE5;
        public const int GL_COLOR_ATTACHMENT6 = 0x8CE6;
        public const int GL_COLOR_ATTACHMENT7 = 0x8CE7;
        public const int GL_COLOR_ATTACHMENT8 = 0x8CE8;
        public const int GL_COLOR_ATTACHMENT9 = 0x8CE9;
        public const int GL_COLOR_ATTACHMENT10 = 0x8CEA;
        public const int GL_COLOR_ATTACHMENT11 = 0x8CEB;
        public const int GL_COLOR_ATTACHMENT12 = 0x8CEC;
        public const int GL_COLOR_ATTACHMENT13 = 0x8CED;
        public const int GL_COLOR_ATTACHMENT14 = 0x8CEE;
        public const int GL_COLOR_ATTACHMENT15 = 0x8CEF;
        public const int GL_DEPTH_ATTACHMENT = 0x8D00;
        public const int GL_STENCIL_ATTACHMENT = 0x8D20;
        public const int GL_DEPTH_STENCIL_ATTACHMENT = 0x821A;
        public const int GL_FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE = 0x8CD0;
        public const int GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME = 0x8CD1;
        public const int GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL = 0x8CD2;
        public const int GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE = 0x8CD3;
        public const int GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER = 0x8CD4;
        public const int GL_FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING = 0x8210;
        public const int GL_FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE = 0x8211;
        public const int GL_FRAMEBUFFER_ATTACHMENT_RED_SIZE = 0x8212;
        public const int GL_FRAMEBUFFER_ATTACHMENT_GREEN_SIZE = 0x8213;
        public const int GL_FRAMEBUFFER_ATTACHMENT_BLUE_SIZE = 0x8214;
        public const int GL_FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE = 0x8215;
        public const int GL_FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE = 0x8216;
        public const int GL_FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE = 0x8217;
        public const int GL_FRAMEBUFFER_DEFAULT = 0x8218;
        public const int GL_RENDERBUFFER_WIDTH = 0x8D42;
        public const int GL_RENDERBUFFER_HEIGHT = 0x8D43;
        public const int GL_RENDERBUFFER_INTERNAL_FORMAT = 0x8D44;
        public const int GL_RENDERBUFFER_RED_SIZE = 0x8D50;
        public const int GL_RENDERBUFFER_GREEN_SIZE = 0x8D51;
        public const int GL_RENDERBUFFER_BLUE_SIZE = 0x8D52;
        public const int GL_RENDERBUFFER_ALPHA_SIZE = 0x8D53;
        public const int GL_RENDERBUFFER_DEPTH_SIZE = 0x8D54;
        public const int GL_RENDERBUFFER_STENCIL_SIZE = 0x8D55;
        public const int GL_DRAW_BUFFER0 = 0x8825;
        public const int GL_DRAW_BUFFER1 = 0x8826;
        public const int GL_DRAW_BUFFER2 = 0x8827;
        public const int GL_DRAW_BUFFER3 = 0x8828;
        public const int GL_DRAW_BUFFER4 = 0x8829;
        public const int GL_DRAW_BUFFER5 = 0x882A;
        public const int GL_DRAW_BUFFER6 = 0x882B;
        public const int GL_DRAW_BUFFER7 = 0x882C;
        public const int GL_DRAW_BUFFER8 = 0x882D;
        public const int GL_DRAW_BUFFER9 = 0x882E;
        public const int GL_DRAW_BUFFER10 = 0x882F;
        public const int GL_DRAW_BUFFER11 = 0x8830;
        public const int GL_DRAW_BUFFER12 = 0x8831;
        public const int GL_DRAW_BUFFER13 = 0x8832;
        public const int GL_DRAW_BUFFER14 = 0x8833;
        public const int GL_DRAW_BUFFER15 = 0x8834;
        public const int GL_READ_BUFFER = 0x0C02;

        // Sync objects
        public const int GL_SYNC_FENCE = 0x9116;
        public const int GL_SYNC_GPU_COMMANDS_COMPLETE = 0x9117;
        public const int GL_SYNC_FLUSH_COMMANDS_BIT = 0x00000001;
        public const int GL_UNSIGNALED = 0x9118;
        public const int GL_SIGNALED = 0x9119;
        public const int GL_ALREADY_SIGNALED = 0x911A;
        public const int GL_TIMEOUT_EXPIRED = 0x911B;
        public const int GL_CONDITION_SATISFIED = 0x911C;
        public const int GL_WAIT_FAILED = 0x911D;
        public const ulong GL_TIMEOUT_IGNORED = 0xFFFFFFFFFFFFFFFFul;
        public const int GL_OBJECT_TYPE = 0x9112;
        public const int GL_SYNC_STATUS = 0x9114;
        public const int GL_SYNC_CONDITION = 0x9113;
        public const int GL_SYNC_FLAGS = 0x9115;
        public const int GL_MAX_SERVER_WAIT_TIMEOUT = 0x9111;

        // Transform feedback
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_MODE = 0x8C7F;
        public const int GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 0x8C80;
        public const int GL_TRANSFORM_FEEDBACK_VARYINGS = 0x8C83;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_START = 0x8C84;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_SIZE = 0x8C85;
        public const int GL_PRIMITIVES_GENERATED = 0x8C87;
        public const int GL_RASTERIZER_DISCARD = 0x8C89;
        public const int GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 0x8C8A;
        public const int GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 0x8C8B;
        public const int GL_INTERLEAVED_ATTRIBS = 0x8C8C;
        public const int GL_SEPARATE_ATTRIBS = 0x8C8D;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_BINDING = 0x8C8F;
        public const int GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 0x8C76;
        public const int GL_TRANSFORM_FEEDBACK = 0x8E22;
        public const int GL_TRANSFORM_FEEDBACK_PAUSED = 0x8E23;
        public const int GL_TRANSFORM_FEEDBACK_ACTIVE = 0x8E24;
        public const int GL_TRANSFORM_FEEDBACK_BINDING = 0x8E25;

        // Uniform buffer objects
        public const int GL_UNIFORM_BUFFER_BINDING = 0x8A28;
        public const int GL_UNIFORM_BUFFER_START = 0x8A29;
        public const int GL_UNIFORM_BUFFER_SIZE = 0x8A2A;
        public const int GL_MAX_VERTEX_UNIFORM_BLOCKS = 0x8A2B;
        public const int GL_MAX_FRAGMENT_UNIFORM_BLOCKS = 0x8A2D;
        public const int GL_MAX_COMBINED_UNIFORM_BLOCKS = 0x8A2E;
        public const int GL_MAX_UNIFORM_BUFFER_BINDINGS = 0x8A2F;
        public const int GL_MAX_UNIFORM_BLOCK_SIZE = 0x8A30;
        public const int GL_MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS = 0x8A31;
        public const int GL_MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS = 0x8A33;
        public const int GL_UNIFORM_BUFFER_OFFSET_ALIGNMENT = 0x8A34;
        public const int GL_ACTIVE_UNIFORM_BLOCK_MAX_NAME_LENGTH = 0x8A35;
        public const int GL_ACTIVE_UNIFORM_BLOCKS = 0x8A36;
        public const int GL_UNIFORM_TYPE = 0x8A37;
        public const int GL_UNIFORM_SIZE = 0x8A38;
        public const int GL_UNIFORM_NAME_LENGTH = 0x8A39;
        public const int GL_UNIFORM_BLOCK_INDEX = 0x8A3A;
        public const int GL_UNIFORM_OFFSET = 0x8A3B;
        public const int GL_UNIFORM_ARRAY_STRIDE = 0x8A3C;
        public const int GL_UNIFORM_MATRIX_STRIDE = 0x8A3D;
        public const int GL_UNIFORM_IS_ROW_MAJOR = 0x8A3E;
        public const int GL_UNIFORM_BLOCK_BINDING = 0x8A3F;
        public const int GL_UNIFORM_BLOCK_DATA_SIZE = 0x8A40;
        public const int GL_UNIFORM_BLOCK_NAME_LENGTH = 0x8A41;
        public const int GL_UNIFORM_BLOCK_ACTIVE_UNIFORMS = 0x8A42;
        public const int GL_UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES = 0x8A43;
        public const int GL_UNIFORM_BLOCK_REFERENCED_BY_VERTEX_SHADER = 0x8A44;
        public const int GL_UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER = 0x8A46;
        public const uint GL_INVALID_INDEX = 0xFFFFFFFFu;

        // Instancing / misc 3.0
        public const int GL_VERTEX_ATTRIB_ARRAY_DIVISOR_ANGLE = 0x88FE;  // same value, alias
        public const int GL_PRIMITIVE_RESTART_FIXED_INDEX = 0x8D69;
        public const int GL_COPY_READ_BUFFER_BINDING = 0x8F36;
        public const int GL_COPY_WRITE_BUFFER_BINDING = 0x8F37;
        public const int GL_UNPACK_COMPRESSED_BLOCK_WIDTH = 0x9127;
        public const int GL_UNPACK_COMPRESSED_BLOCK_HEIGHT = 0x9128;
        public const int GL_UNPACK_COMPRESSED_BLOCK_DEPTH = 0x9129;
        public const int GL_UNPACK_COMPRESSED_BLOCK_SIZE = 0x912A;
        public const int GL_PACK_COMPRESSED_BLOCK_WIDTH = 0x912B;
        public const int GL_PACK_COMPRESSED_BLOCK_HEIGHT = 0x912C;
        public const int GL_PACK_COMPRESSED_BLOCK_DEPTH = 0x912D;
        public const int GL_PACK_COMPRESSED_BLOCK_SIZE = 0x912E;
        public const int GL_MIN_PROGRAM_TEXEL_OFFSET = 0x8904;
        public const int GL_MAX_PROGRAM_TEXEL_OFFSET = 0x8905;
        public const int GL_MAJOR_VERSION = 0x821B;
        public const int GL_MINOR_VERSION = 0x821C;
        public const int GL_CONTEXT_FLAGS = 0x821E;
        public const int GL_UNSIGNED_NORMALIZED = 0x8C17;
        public const int GL_SIGNED_NORMALIZED = 0x8F9C;
        public const int GL_TEXTURE_IMMUTABLE_FORMAT = 0x912F;
        public const int GL_SAMPLER_BINDING = 0x8919;
        public const int GL_MAX_DUAL_SOURCE_DRAW_BUFFERS = 0x88FC;
        public const int GL_STENCIL_BACK_FUNC = 0x8800;
        public const int GL_STENCIL_BACK_FAIL = 0x8801;
        public const int GL_STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802;
        public const int GL_STENCIL_BACK_PASS_DEPTH_PASS = 0x8803;
        public const int GL_STENCIL_BACK_REF = 0x8CA3;
        public const int GL_STENCIL_BACK_VALUE_MASK = 0x8CA4;
        public const int GL_STENCIL_BACK_WRITEMASK = 0x8CA5;
        public const int GL_IMPLEMENTATION_COLOR_READ_TYPE = 0x8B9A;
        public const int GL_IMPLEMENTATION_COLOR_READ_FORMAT = 0x8B9B;

        #endregion
    }
}