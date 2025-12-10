using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenGL.ES
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public static unsafe class GLES30
    {
#if ANDROID
    public const string LIB = "libGLESv2.so";
#elif IOS || __IOS__
    public const string LIB = "__Internal";
#else
        public const string LIB = "GLESv2";
#endif
        private static string PtrToStringUtf8(IntPtr ptr)
        {
            var length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
                length++;
            var buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }
        private static string PtrToStringUtf8(IntPtr ptr, int length)
        {
            var buffer = new byte[length];
            Marshal.Copy(ptr, buffer, 0, length);
            return Encoding.UTF8.GetString(buffer);
        }

        [DllImport(LIB, EntryPoint = "glActiveTexture")]
        public static extern void glActiveTexture(int texture);

        [DllImport(LIB, EntryPoint = "glAttachShader")]
        public static extern void glAttachShader(uint program, uint shader);

        [DllImport(LIB, EntryPoint = "glBindAttribLocation")]
        public static extern void glBindAttribLocation(int program, int index, string name);

        [DllImport(LIB, EntryPoint = "glBindBuffer")]
        public static extern void glBindBuffer(int target, uint buffer);

        [DllImport(LIB, EntryPoint = "glBindFramebuffer")]
        public static extern void glBindFramebuffer(int target, uint framebuffer);

        [DllImport(LIB, EntryPoint = "glBindRenderbuffer")]
        public static extern void glBindRenderbuffer(int target, uint renderbuffer);
        public static void glBindRenderbuffer(uint renderbuffer)
        {
            glBindRenderbuffer(GL_RENDERBUFFER, renderbuffer);
        }

        [DllImport(LIB, EntryPoint = "glBindTexture")]
        public static extern void glBindTexture(int target, uint texture);

        [DllImport(LIB, EntryPoint = "glBlendColor")]
        public static extern void glBlendColor(float red, float green, float blue, float alpha);

        [DllImport(LIB, EntryPoint = "glBlendEquation")]
        public static extern void glBlendEquation(int mode);

        [DllImport(LIB, EntryPoint = "glBlendEquationSeparate")]
        public static extern void glBlendEquationSeparate(int modeRGB, int modeAlpha);

        [DllImport(LIB, EntryPoint = "glBlendFunc")]
        public static extern void glBlendFunc(int sfactor, int dfactor);

        [DllImport(LIB, EntryPoint = "glBlendFuncSeparate")]
        public static extern void glBlendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha);

        [DllImport(LIB, EntryPoint = "glBufferData")]
        public static extern void glBufferData(int target, IntPtr size, void* data, int usage);

        [DllImport(LIB, EntryPoint = "glBufferSubData")]
        public static extern void glBufferSubData(int target, IntPtr offset, IntPtr size, void* data);

        [DllImport(LIB, EntryPoint = "glCheckFramebufferStatus")]
        public static extern int glCheckFramebufferStatus(int target);

        [DllImport(LIB, EntryPoint = "glClear")]
        public static extern void glClear(uint mask);

        [DllImport(LIB, EntryPoint = "glClearColor")]
        public static extern void glClearColor(float red, float green, float blue, float alpha);

        [DllImport(LIB, EntryPoint = "glClearDepthf")]
        public static extern void glClearDepthf(float d);

        [DllImport(LIB, EntryPoint = "glClearStencil")]
        public static extern void glClearStencil(int s);

        [DllImport(LIB, EntryPoint = "glColorMask")]
        public static extern void glColorMask(bool red, bool green, bool blue, bool alpha);

        [DllImport(LIB, EntryPoint = "glCompileShader")]
        public static extern void glCompileShader(uint shader);

        [DllImport(LIB, EntryPoint = "glCompressedTexImage2D")]
        public static extern void glCompressedTexImage2D(int target, int level, int internalformat, int width, int height, int border, int imageSize, void* data);

        [DllImport(LIB, EntryPoint = "glCompressedTexSubImage2D")]
        public static extern void glCompressedTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int imageSize, void* data);

        [DllImport(LIB, EntryPoint = "glCopyTexImage2D")]
        public static extern void glCopyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height, int border);

        [DllImport(LIB, EntryPoint = "glCopyTexSubImage2D")]
        public static extern void glCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width, int height);

        [DllImport(LIB, EntryPoint = "glCreateProgram")]
        public static extern uint glCreateProgram();

        [DllImport(LIB, EntryPoint = "glCreateShader")]
        public static extern uint glCreateShader(int type);

        [DllImport(LIB, EntryPoint = "glCullFace")]
        public static extern void glCullFace(int mode);

        [DllImport(LIB, EntryPoint = "glDeleteBuffers")]
        public static extern void glDeleteBuffers(int n, uint* buffers);

        public static void glDeleteBuffer(uint buffer)
        {
            glDeleteBuffers(1, &buffer);
        }

        [DllImport(LIB, EntryPoint = "glDeleteFramebuffers")]
        public static extern void glDeleteFramebuffers(int n, uint* framebuffers);
        public static void glDeleteFramebuffer(uint framebuffer)
        {
            glDeleteFramebuffers(1, &framebuffer);
        }

        [DllImport(LIB, EntryPoint = "glDeleteProgram")]
        public static extern void glDeleteProgram(uint program);

        [DllImport(LIB, EntryPoint = "glDeleteRenderbuffers")]
        public static extern void glDeleteRenderbuffers(int n, uint* renderbuffers);
        public static void glDeleteRenderbuffer(uint renderbuffer)
        {
            glDeleteRenderbuffers(1, &renderbuffer);
        }

        [DllImport(LIB, EntryPoint = "glDeleteShader")]
        public static extern void glDeleteShader(uint shader);

        [DllImport(LIB, EntryPoint = "glDeleteTextures")]
        public static extern void glDeleteTextures(int n, uint* textures);
        public static void glDeleteTexture(uint texture)
        {
            glDeleteTextures(1, &texture);
        }
        [DllImport(LIB, EntryPoint = "glDepthFunc")]
        public static extern void glDepthFunc(int func);

        [DllImport(LIB, EntryPoint = "glDepthMask")]
        public static extern void glDepthMask(bool flag);

        [DllImport(LIB, EntryPoint = "glDepthRangef")]
        public static extern void glDepthRangef(float n, float f);

        [DllImport(LIB, EntryPoint = "glDetachShader")]
        public static extern void glDetachShader(int program, int shader);

        [DllImport(LIB, EntryPoint = "glDisable")]
        public static extern void glDisable(int cap);

        [DllImport(LIB, EntryPoint = "glDisableVertexAttribArray")]
        public static extern void glDisableVertexAttribArray(int index);

        [DllImport(LIB, EntryPoint = "glDrawArrays")]
        public static extern void glDrawArrays(int mode, int first, int count);

        [DllImport(LIB, EntryPoint = "glDrawElements")]
        public static extern void glDrawElements(int mode, int count, int type, void* indices);

        [DllImport(LIB, EntryPoint = "glEnable")]
        public static extern void glEnable(int cap);

        [DllImport(LIB, EntryPoint = "glEnableVertexAttribArray")]
        public static extern void glEnableVertexAttribArray(uint index);

        [DllImport(LIB, EntryPoint = "glFinish")]
        public static extern void glFinish();

        [DllImport(LIB, EntryPoint = "glFlush")]
        public static extern void glFlush();

        [DllImport(LIB, EntryPoint = "glFramebufferRenderbuffer")]
        public static extern void glFramebufferRenderbuffer(int target, int attachment, int renderbuffertarget, uint renderbuffer);

        [DllImport(LIB, EntryPoint = "glFramebufferTexture2D")]
        public static extern void glFramebufferTexture2D(int target, int attachment, int textarget, uint texture, int level);

        [DllImport(LIB, EntryPoint = "glFrontFace")]
        public static extern void glFrontFace(int mode);

        [DllImport(LIB, EntryPoint = "glGenBuffers")]
        public static extern void glGenBuffers(int n, uint* buffers);

        public static uint glGenBuffer()
        {
            uint id;
            glGenBuffers(1, &id);
            return id;
        }

        [DllImport(LIB, EntryPoint = "glGenerateMipmap")]
        public static extern void glGenerateMipmap(int target);

        [DllImport(LIB, EntryPoint = "glGenFramebuffers")]
        public static extern void glGenFramebuffers(int n, uint* framebuffers);
        public static uint glGenFramebuffer()
        {
            uint id = 0;
            glGenFramebuffers(1, &id);
            return id;
        }

        [DllImport(LIB, EntryPoint = "glGenRenderbuffers")]
        public static extern void glGenRenderbuffers(int n, uint* renderbuffers);
        public static uint glGenRenderbuffer()
        {
            uint id = 0;
            glGenRenderbuffers(1, &id);
            return id;
        }

        [DllImport(LIB, EntryPoint = "glGenTextures")]
        public static extern void glGenTextures(int n, uint* textures);

        public static uint glGenTexture()
        {
            uint id = 0;
            glGenTextures(1, &id);

            return id;
        }
        [DllImport(LIB, EntryPoint = "glGetActiveAttrib")]
        public static extern void glGetActiveAttrib(int program, int index, int bufSize, int* length, int* size, int* type, byte* name);

        [DllImport(LIB, EntryPoint = "glGetActiveUniform")]
        public static extern void glGetActiveUniform(int program, int index, int bufSize, int* length, int* size, int* type, byte* name);

        [DllImport(LIB, EntryPoint = "glGetAttachedShaders")]
        public static extern void glGetAttachedShaders(int program, int maxCount, int* count, int* shaders);

        [DllImport(LIB, EntryPoint = "glGetAttribLocation")]
        public static extern int glGetAttribLocation(int program, string name);

        [DllImport(LIB, EntryPoint = "glGetBooleanv")]
        public static extern void glGetBooleanv(int pname, byte* data);

        [DllImport(LIB, EntryPoint = "glGetBufferParameteriv")]
        public static extern void glGetBufferParameteriv(int target, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetError")]
        public static extern int glGetError();

        [DllImport(LIB, EntryPoint = "glGetFloatv")]
        public static extern void glGetFloatv(int pname, float* data);

        [DllImport(LIB, EntryPoint = "glGetFramebufferAttachmentParameteriv")]
        public static extern void glGetFramebufferAttachmentParameteriv(int target, int attachment, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetIntegerv")]
        public static extern void glGetIntegerv(int pname, int* data);

        [DllImport(LIB, EntryPoint = "glGetProgramiv")]
        public static extern void glGetProgramiv(uint program, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetProgramInfoLog")]
        public static extern void glGetProgramInfoLog(uint program, int bufSize, int* length, byte* infoLog);

        public static string glGetProgramInfoLog(uint program, int bufSize = 1024)
        {
            var buffer = Marshal.AllocHGlobal(bufSize);
            try
            {
                int length;
                var source = (byte*)buffer.ToPointer();
                glGetProgramInfoLog(program, bufSize, &length, source);
                return PtrToStringUtf8(buffer, length);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        [DllImport(LIB, EntryPoint = "glGetRenderbufferParameteriv")]
        public static extern void glGetRenderbufferParameteriv(int target, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetShaderiv")]
        public static extern void glGetShaderiv(uint shader, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetShaderInfoLog")]
        public static extern void glGetShaderInfoLog(uint shader, int bufSize, int* length, byte* infoLog);

        public static string glGetShaderInfoLog(uint shader, int bufSize = 1024)
        {
            var buffer = Marshal.AllocHGlobal(bufSize);
            try
            {
                int length;
                var source = (byte*)buffer.ToPointer();
                glGetShaderInfoLog(shader, bufSize, &length, source);
                return PtrToStringUtf8(buffer, length);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        [DllImport(LIB, EntryPoint = "glGetShaderPrecisionFormat")]
        public static extern void glGetShaderPrecisionFormat(int shadertype, int precisiontype, int* range, int* precision);

        [DllImport(LIB, EntryPoint = "glGetShaderSource")]
        public static extern void glGetShaderSource(int shader, int bufSize, int* length, byte* source);

        [DllImport(LIB, EntryPoint = "glGetString")]
        private static extern IntPtr _glGetString(int name);

        public static string glGetString(int name)
        {
            return PtrToStringUtf8(new IntPtr(_glGetString(name)));
        }


        [DllImport(LIB, EntryPoint = "glGetTexParameterfv")]
        public static extern void glGetTexParameterfv(int target, int pname, float* @params);

        [DllImport(LIB, EntryPoint = "glGetTexParameteriv")]
        public static extern void glGetTexParameteriv(int target, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetUniformfv")]
        public static extern void glGetUniformfv(int program, int location, float* @params);

        [DllImport(LIB, EntryPoint = "glGetUniformiv")]
        public static extern void glGetUniformiv(int program, int location, int* @params);

        [DllImport(LIB, EntryPoint = "glGetUniformLocation")]
        public static extern int glGetUniformLocation(uint program, string name);

        [DllImport(LIB, EntryPoint = "glGetVertexAttribfv")]
        public static extern void glGetVertexAttribfv(int index, int pname, float* @params);

        [DllImport(LIB, EntryPoint = "glGetVertexAttribiv")]
        public static extern void glGetVertexAttribiv(int index, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetVertexAttribPointerv")]
        public static extern void glGetVertexAttribPointerv(int index, int pname, void** pointer);

        [DllImport(LIB, EntryPoint = "glHint")]
        public static extern void glHint(int target, int mode);

        [DllImport(LIB, EntryPoint = "glIsBuffer")]
        public static extern bool glIsBuffer(int buffer);

        [DllImport(LIB, EntryPoint = "glIsEnabled")]
        public static extern bool glIsEnabled(int cap);

        [DllImport(LIB, EntryPoint = "glIsFramebuffer")]
        public static extern bool glIsFramebuffer(int framebuffer);

        [DllImport(LIB, EntryPoint = "glIsProgram")]
        public static extern bool glIsProgram(int program);

        [DllImport(LIB, EntryPoint = "glIsRenderbuffer")]
        public static extern bool glIsRenderbuffer(int renderbuffer);

        [DllImport(LIB, EntryPoint = "glIsShader")]
        public static extern bool glIsShader(int shader);

        [DllImport(LIB, EntryPoint = "glIsTexture")]
        public static extern bool glIsTexture(int texture);

        [DllImport(LIB, EntryPoint = "glLineWidth")]
        public static extern void glLineWidth(float width);

        [DllImport(LIB, EntryPoint = "glLinkProgram")]
        public static extern void glLinkProgram(uint program);

        [DllImport(LIB, EntryPoint = "glPixelStorei")]
        public static extern void glPixelStorei(int pname, int param);

        [DllImport(LIB, EntryPoint = "glPolygonOffset")]
        public static extern void glPolygonOffset(float factor, float units);

        [DllImport(LIB, EntryPoint = "glReadPixels")]
        public static extern void glReadPixels(int x, int y, int width, int height, int format, int type, void* pixels);
        public static void glReadPixels(int x, int y, int width, int height, int format, int type, IntPtr pixels)
        {
            glReadPixels(x, y, width, height, format, type, new IntPtr(pixels).ToPointer());
        }

        [DllImport(LIB, EntryPoint = "glReleaseShaderCompiler")]
        public static extern void glReleaseShaderCompiler();

        [DllImport(LIB, EntryPoint = "glRenderbufferStorage")]
        public static extern void glRenderbufferStorage(int target, int internalformat, int width, int height);

        [DllImport(LIB, EntryPoint = "glSampleCoverage")]
        public static extern void glSampleCoverage(float value, bool invert);

        [DllImport(LIB, EntryPoint = "glScissor")]
        public static extern void glScissor(int x, int y, int width, int height);

        [DllImport(LIB, EntryPoint = "glShaderBinary")]
        public static extern void glShaderBinary(int count, int* shaders, int binaryformat, void* binary, int length);

        [DllImport(LIB, EntryPoint = "glShaderSource")]
        private static extern void glShaderSource_(uint shader, int count, byte** strings, int* lengths);

        public static unsafe void glShaderSource(uint shader, string src)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(src + "\0");
            fixed (byte* pUtf8 = utf8)
            {
                var ptr = pUtf8;
                int len = utf8.Length;
                glShaderSource_(shader, 1, &ptr, &len);
            }
        }


        [DllImport(LIB, EntryPoint = "glStencilFunc")]
        public static extern void glStencilFunc(int func, int @ref, uint mask);

        [DllImport(LIB, EntryPoint = "glStencilFuncSeparate")]
        public static extern void glStencilFuncSeparate(int face, int func, int @ref, int mask);

        [DllImport(LIB, EntryPoint = "glStencilMask")]
        public static extern void glStencilMask(int mask);

        [DllImport(LIB, EntryPoint = "glStencilMaskSeparate")]
        public static extern void glStencilMaskSeparate(int face, int mask);

        [DllImport(LIB, EntryPoint = "glStencilOp")]
        public static extern void glStencilOp(int fail, int zfail, int zpass);

        [DllImport(LIB, EntryPoint = "glStencilOpSeparate")]
        public static extern void glStencilOpSeparate(int face, int sfail, int dpfail, int dppass);

        [DllImport(LIB, EntryPoint = "glTexImage2D")]
        public static extern void glTexImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, void* data);

        [DllImport(LIB, EntryPoint = "glTexParameterf")]
        public static extern void glTexParameterf(int target, int pname, float param);

        [DllImport(LIB, EntryPoint = "glTexParameterfv")]
        public static extern void glTexParameterfv(int target, int pname, float* @params);

        [DllImport(LIB, EntryPoint = "glTexParameteri")]
        public static extern void glTexParameteri(int target, int pname, int param);

        [DllImport(LIB, EntryPoint = "glTexParameteriv")]
        public static extern void glTexParameteriv(int target, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glTexSubImage2D")]
        public static extern void glTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int type, void* data);

        [DllImport(LIB, EntryPoint = "glUniform1f")]
        public static extern void glUniform1f(int location, float v0);

        [DllImport(LIB, EntryPoint = "glUniform1fv")]
        public static extern void glUniform1fv(int location, int count, float* value);

        [DllImport(LIB, EntryPoint = "glUniform1i")]
        public static extern void glUniform1i(int location, int v0);

        [DllImport(LIB, EntryPoint = "glUniform1iv")]
        public static extern void glUniform1iv(int location, int count, int* value);

        [DllImport(LIB, EntryPoint = "glUniform2f")]
        public static extern void glUniform2f(int location, float v0, float v1);

        [DllImport(LIB, EntryPoint = "glUniform2fv")]
        public static extern void glUniform2fv(int location, int count, float* value);

        [DllImport(LIB, EntryPoint = "glUniform2i")]
        public static extern void glUniform2i(int location, int v0, int v1);

        [DllImport(LIB, EntryPoint = "glUniform2iv")]
        public static extern void glUniform2iv(int location, int count, int* value);

        [DllImport(LIB, EntryPoint = "glUniform3f")]
        public static extern void glUniform3f(int location, float v0, float v1, float v2);

        [DllImport(LIB, EntryPoint = "glUniform3fv")]
        public static extern void glUniform3fv(int location, int count, float* value);

        [DllImport(LIB, EntryPoint = "glUniform3i")]
        public static extern void glUniform3i(int location, int v0, int v1, int v2);

        [DllImport(LIB, EntryPoint = "glUniform3iv")]
        public static extern void glUniform3iv(int location, int count, int* value);

        [DllImport(LIB, EntryPoint = "glUniform4f")]
        public static extern void glUniform4f(int location, float v0, float v1, float v2, float v3);

        [DllImport(LIB, EntryPoint = "glUniform4fv")]
        public static extern void glUniform4fv(int location, int count, float* value);

        [DllImport(LIB, EntryPoint = "glUniform4i")]
        public static extern void glUniform4i(int location, int v0, int v1, int v2, int v3);

        [DllImport(LIB, EntryPoint = "glUniform4iv")]
        public static extern void glUniform4iv(int location, int count, int* value);

        [DllImport(LIB, EntryPoint = "glUniformMatrix2fv")]
        public static extern void glUniformMatrix2fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glUniformMatrix3fv")]
        public static extern void glUniformMatrix3fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glUniformMatrix4fv")]
        public static extern void glUniformMatrix4fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glUseProgram")]
        public static extern void glUseProgram(uint program);

        [DllImport(LIB, EntryPoint = "glValidateProgram")]
        public static extern void glValidateProgram(uint program);

        [DllImport(LIB, EntryPoint = "glVertexAttrib1f")]
        public static extern void glVertexAttrib1f(int index, float x);

        [DllImport(LIB, EntryPoint = "glVertexAttrib1fv")]
        public static extern void glVertexAttrib1fv(int index, float* v);

        [DllImport(LIB, EntryPoint = "glVertexAttrib2f")]
        public static extern void glVertexAttrib2f(int index, float x, float y);

        [DllImport(LIB, EntryPoint = "glVertexAttrib2fv")]
        public static extern void glVertexAttrib2fv(int index, float* v);

        [DllImport(LIB, EntryPoint = "glVertexAttrib3f")]
        public static extern void glVertexAttrib3f(int index, float x, float y, float z);

        [DllImport(LIB, EntryPoint = "glVertexAttrib3fv")]
        public static extern void glVertexAttrib3fv(int index, float* v);

        [DllImport(LIB, EntryPoint = "glVertexAttrib4f")]
        public static extern void glVertexAttrib4f(int index, float x, float y, float z, float w);

        [DllImport(LIB, EntryPoint = "glVertexAttrib4fv")]
        public static extern void glVertexAttrib4fv(int index, float* v);

        [DllImport(LIB, EntryPoint = "glVertexAttribPointer")]
        public static extern void glVertexAttribPointer(uint index, int size, int type, bool normalized, int stride, void* pointer);
        public static void glVertexAttribPointer(uint index, int size, int type, bool normalized, int stride, IntPtr pointer)
        {
            glVertexAttribPointer(index, size, type, normalized, stride, pointer.ToPointer());
        }

        [DllImport(LIB, EntryPoint = "glViewport")]
        public static extern void glViewport(int x, int y, int width, int height);

        [DllImport(LIB, EntryPoint = "glReadBuffer")]
        public static extern void glReadBuffer(int mode);

        [DllImport(LIB, EntryPoint = "glDrawRangeElements")]
        public static extern void glDrawRangeElements(int mode, int start, int end, int count, int type, void* indices);

        [DllImport(LIB, EntryPoint = "glTexImage3D")]
        public static extern void glTexImage3D(int target, int level, int internalformat, int width, int height, int depth, int border, int format, int type, void* data);

        [DllImport(LIB, EntryPoint = "glTexSubImage3D")]
        public static extern void glTexSubImage3D(int target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, int format, int type, void* data);

        [DllImport(LIB, EntryPoint = "glCopyTexSubImage3D")]
        public static extern void glCopyTexSubImage3D(int target, int level, int xoffset, int yoffset, int zoffset, int x, int y, int width, int height);

        [DllImport(LIB, EntryPoint = "glCompressedTexImage3D")]
        public static extern void glCompressedTexImage3D(int target, int level, int internalformat, int width, int height, int depth, int border, int imageSize, void* data);

        [DllImport(LIB, EntryPoint = "glCompressedTexSubImage3D")]
        public static extern void glCompressedTexSubImage3D(int target, int level, int xoffset, int yoffset, int zoffset, int width, int height, int depth, int format, int imageSize, void* data);

        [DllImport(LIB, EntryPoint = "glGenQueries")]
        public static extern void glGenQueries(int n, int* ids);

        [DllImport(LIB, EntryPoint = "glDeleteQueries")]
        public static extern void glDeleteQueries(int n, int* ids);

        [DllImport(LIB, EntryPoint = "glIsQuery")]
        public static extern bool glIsQuery(int id);

        [DllImport(LIB, EntryPoint = "glBeginQuery")]
        public static extern void glBeginQuery(int target, int id);

        [DllImport(LIB, EntryPoint = "glEndQuery")]
        public static extern void glEndQuery(int target);

        [DllImport(LIB, EntryPoint = "glGetQueryiv")]
        public static extern void glGetQueryiv(int target, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetQueryObjectuiv")]
        public static extern void glGetQueryObjectuiv(int id, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glUnmapBuffer")]
        public static extern bool glUnmapBuffer(int target);

        [DllImport(LIB, EntryPoint = "glGetBufferPointerv")]
        public static extern void glGetBufferPointerv(int target, int pname, void** @params);

        [DllImport(LIB, EntryPoint = "glDrawBuffers")]
        public static extern void glDrawBuffers(int n, int* bufs);

        [DllImport(LIB, EntryPoint = "glUniformMatrix2x3fv")]
        public static extern void glUniformMatrix2x3fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glUniformMatrix3x2fv")]
        public static extern void glUniformMatrix3x2fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glUniformMatrix2x4fv")]
        public static extern void glUniformMatrix2x4fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glUniformMatrix4x2fv")]
        public static extern void glUniformMatrix4x2fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glUniformMatrix3x4fv")]
        public static extern void glUniformMatrix3x4fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glUniformMatrix4x3fv")]
        public static extern void glUniformMatrix4x3fv(int location, int count, bool transpose, float* value);

        [DllImport(LIB, EntryPoint = "glBlitFramebuffer")]
        public static extern void glBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, uint mask, int filter);

        [DllImport(LIB, EntryPoint = "glRenderbufferStorageMultisample")]
        public static extern void glRenderbufferStorageMultisample(int target, int samples, int internalformat, int width, int height);

        [DllImport(LIB, EntryPoint = "glFramebufferTextureLayer")]
        public static extern void glFramebufferTextureLayer(int target, int attachment, int texture, int level, int layer);

        [DllImport(LIB, EntryPoint = "glMapBufferRange")]
        public static extern void* glMapBufferRange(int target, IntPtr offset, IntPtr length, int access);

        [DllImport(LIB, EntryPoint = "glFlushMappedBufferRange")]
        public static extern void glFlushMappedBufferRange(int target, IntPtr offset, IntPtr length);

        [DllImport(LIB, EntryPoint = "glBindVertexArray")]
        public static extern void glBindVertexArray(uint array);

        [DllImport(LIB, EntryPoint = "glDeleteVertexArrays")]
        public static extern void glDeleteVertexArrays(int n, uint* arrays);
        public static void glDeleteVertexArray(uint array)
        {
            glDeleteVertexArrays(1, &array);
        }

        [DllImport(LIB, EntryPoint = "glGenVertexArrays")]
        public static extern void glGenVertexArrays(int n, uint* arrays);
        public static uint glGenVertexArray()
        {
            uint id = 0;
            glGenVertexArrays(1, &id);
            return id;
        }

        [DllImport(LIB, EntryPoint = "glIsVertexArray")]
        public static extern bool glIsVertexArray(int array);

        [DllImport(LIB, EntryPoint = "glGetIntegeri_v")]
        public static extern void glGetIntegeri_v(int target, int index, int* data);

        [DllImport(LIB, EntryPoint = "glBeginTransformFeedback")]
        public static extern void glBeginTransformFeedback(int primitiveMode);

        [DllImport(LIB, EntryPoint = "glEndTransformFeedback")]
        public static extern void glEndTransformFeedback();

        [DllImport(LIB, EntryPoint = "glBindBufferRange")]
        public static extern void glBindBufferRange(int target, int index, int buffer, IntPtr offset, IntPtr size);

        [DllImport(LIB, EntryPoint = "glBindBufferBase")]
        public static extern void glBindBufferBase(int target, int index, int buffer);

        [DllImport(LIB, EntryPoint = "glTransformFeedbackVaryings")]
        public static extern void glTransformFeedbackVaryings(int program, int count, string[] varyings, int bufferMode);

        [DllImport(LIB, EntryPoint = "glGetTransformFeedbackVarying")]
        public static extern void glGetTransformFeedbackVarying(int program, int index, int bufSize, int* length, int* size, int* type, byte* name);

        [DllImport(LIB, EntryPoint = "glVertexAttribIPointer")]
        public static extern void glVertexAttribIPointer(uint index, int size, int type, int stride, void* pointer);
        public static void glVertexAttribIPointer(uint index, int size, int type, int stride, IntPtr pointer)
        {
            glVertexAttribIPointer(index, size, type, stride, pointer.ToPointer());
        }
        [DllImport(LIB, EntryPoint = "glGetVertexAttribIiv")]
        public static extern void glGetVertexAttribIiv(int index, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetVertexAttribIuiv")]
        public static extern void glGetVertexAttribIuiv(int index, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glVertexAttribI4i")]
        public static extern void glVertexAttribI4i(int index, int x, int y, int z, int w);

        [DllImport(LIB, EntryPoint = "glVertexAttribI4ui")]
        public static extern void glVertexAttribI4ui(int index, int x, int y, int z, int w);

        [DllImport(LIB, EntryPoint = "glVertexAttribI4iv")]
        public static extern void glVertexAttribI4iv(int index, int* v);

        [DllImport(LIB, EntryPoint = "glVertexAttribI4uiv")]
        public static extern void glVertexAttribI4uiv(int index, int* v);

        [DllImport(LIB, EntryPoint = "glGetUniformuiv")]
        public static extern void glGetUniformuiv(int program, int location, int* @params);

        [DllImport(LIB, EntryPoint = "glGetFragDataLocation")]
        public static extern int glGetFragDataLocation(int program, string name);

        [DllImport(LIB, EntryPoint = "glUniform1ui")]
        public static extern void glUniform1ui(int location, uint v0);

        [DllImport(LIB, EntryPoint = "glUniform2ui")]
        public static extern void glUniform2ui(int location, int v0, int v1);

        [DllImport(LIB, EntryPoint = "glUniform3ui")]
        public static extern void glUniform3ui(int location, int v0, int v1, int v2);

        [DllImport(LIB, EntryPoint = "glUniform4ui")]
        public static extern void glUniform4ui(int location, int v0, int v1, int v2, int v3);

        [DllImport(LIB, EntryPoint = "glUniform1uiv")]
        public static extern void glUniform1uiv(int location, int count, int* value);

        [DllImport(LIB, EntryPoint = "glUniform2uiv")]
        public static extern void glUniform2uiv(int location, int count, int* value);

        [DllImport(LIB, EntryPoint = "glUniform3uiv")]
        public static extern void glUniform3uiv(int location, int count, int* value);

        [DllImport(LIB, EntryPoint = "glUniform4uiv")]
        public static extern void glUniform4uiv(int location, int count, int* value);

        [DllImport(LIB, EntryPoint = "glFenceSync")]
        public static extern IntPtr glFenceSync(int condition, int flags);

        [DllImport(LIB, EntryPoint = "glIsSync")]
        public static extern bool glIsSync(IntPtr sync);

        [DllImport(LIB, EntryPoint = "glDeleteSync")]
        public static extern void glDeleteSync(IntPtr sync);

        [DllImport(LIB, EntryPoint = "glClientWaitSync")]
        public static extern int glClientWaitSync(IntPtr sync, int flags, ulong timeout);

        [DllImport(LIB, EntryPoint = "glWaitSync")]
        public static extern void glWaitSync(IntPtr sync, int flags, ulong timeout);

        [DllImport(LIB, EntryPoint = "glGetInteger64v")]
        public static extern void glGetInteger64v(int pname, long* data);

        [DllImport(LIB, EntryPoint = "glGetSynciv")]
        public static extern void glGetSynciv(IntPtr sync, int pname, int bufSize, int* length, int* values);

        [DllImport(LIB, EntryPoint = "glGetInteger64i_v")]
        public static extern void glGetInteger64i_v(int target, int index, long* data);

        [DllImport(LIB, EntryPoint = "glGetBufferParameteri64v")]
        public static extern void glGetBufferParameteri64v(int target, int pname, long* @params);

        [DllImport(LIB, EntryPoint = "glGenSamplers")]
        public static extern void glGenSamplers(int count, int* samplers);

        [DllImport(LIB, EntryPoint = "glDeleteSamplers")]
        public static extern void glDeleteSamplers(int count, int* samplers);

        [DllImport(LIB, EntryPoint = "glIsSampler")]
        public static extern bool glIsSampler(int sampler);

        [DllImport(LIB, EntryPoint = "glBindSampler")]
        public static extern void glBindSampler(int unit, int sampler);

        [DllImport(LIB, EntryPoint = "glSamplerParameteri")]
        public static extern void glSamplerParameteri(int sampler, int pname, int param);

        [DllImport(LIB, EntryPoint = "glSamplerParameteriv")]
        public static extern void glSamplerParameteriv(int sampler, int pname, int* param);

        [DllImport(LIB, EntryPoint = "glSamplerParameterf")]
        public static extern void glSamplerParameterf(int sampler, int pname, float param);

        [DllImport(LIB, EntryPoint = "glSamplerParameterfv")]
        public static extern void glSamplerParameterfv(int sampler, int pname, float* param);

        [DllImport(LIB, EntryPoint = "glGetSamplerParameteriv")]
        public static extern void glGetSamplerParameteriv(int sampler, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetSamplerParameterfv")]
        public static extern void glGetSamplerParameterfv(int sampler, int pname, float* @params);

        [DllImport(LIB, EntryPoint = "glVertexAttribDivisor")]
        public static extern void glVertexAttribDivisor(int index, int divisor);

        [DllImport(LIB, EntryPoint = "glBindTransformFeedback")]
        public static extern void glBindTransformFeedback(int target, int id);

        [DllImport(LIB, EntryPoint = "glDeleteTransformFeedbacks")]
        public static extern void glDeleteTransformFeedbacks(int n, int* ids);

        [DllImport(LIB, EntryPoint = "glGenTransformFeedbacks")]
        public static extern void glGenTransformFeedbacks(int n, int* ids);

        [DllImport(LIB, EntryPoint = "glIsTransformFeedback")]
        public static extern bool glIsTransformFeedback(int id);

        [DllImport(LIB, EntryPoint = "glPauseTransformFeedback")]
        public static extern void glPauseTransformFeedback();

        [DllImport(LIB, EntryPoint = "glResumeTransformFeedback")]
        public static extern void glResumeTransformFeedback();

        [DllImport(LIB, EntryPoint = "glGetProgramBinary")]
        public static extern void glGetProgramBinary(int program, int bufSize, int* length, int* binaryFormat, void* binary);

        [DllImport(LIB, EntryPoint = "glProgramBinary")]
        public static extern void glProgramBinary(int program, int binaryFormat, void* binary, int length);

        [DllImport(LIB, EntryPoint = "glProgramParameteri")]
        public static extern void glProgramParameteri(int program, int pname, int value);

        [DllImport(LIB, EntryPoint = "glInvalidateFramebuffer")]
        public static extern void glInvalidateFramebuffer(int target, int numAttachments, int* attachments);

        [DllImport(LIB, EntryPoint = "glInvalidateSubFramebuffer")]
        public static extern void glInvalidateSubFramebuffer(int target, int numAttachments, int* attachments, int x, int y, int width, int height);

        [DllImport(LIB, EntryPoint = "glTexStorage2D")]
        public static extern void glTexStorage2D(int target, int levels, int internalformat, int width, int height);

        [DllImport(LIB, EntryPoint = "glTexStorage3D")]
        public static extern void glTexStorage3D(int target, int levels, int internalformat, int width, int height, int depth);

        [DllImport(LIB, EntryPoint = "glGetInternalformativ")]
        public static extern void glGetInternalformativ(int target, int internalformat, int pname, int bufSize, int* @params);

        [DllImport(LIB, EntryPoint = "glDrawArraysInstanced")]
        public static extern void glDrawArraysInstanced(int mode, int first, int count, int instancecount);

        [DllImport(LIB, EntryPoint = "glDrawElementsInstanced")]
        public static extern void glDrawElementsInstanced(int mode, int count, int type, void* indices, int instancecount);


        #region Consts
        public const int GL_DEPTH_BUFFER_BIT = 0x00000100;
        public const int GL_STENCIL_BUFFER_BIT = 0x00000400;
        public const int GL_COLOR_BUFFER_BIT = 0x00004000;
        public const int GL_FALSE = 0;
        public const int GL_TRUE = 1;
        public const int GL_POINTS = 0x0000;
        public const int GL_LINES = 0x0001;
        public const int GL_LINE_LOOP = 0x0002;
        public const int GL_LINE_STRIP = 0x0003;
        public const int GL_TRIANGLES = 0x0004;
        public const int GL_TRIANGLE_STRIP = 0x0005;
        public const int GL_TRIANGLE_FAN = 0x0006;
        public const int GL_NEVER = 0x0200;
        public const int GL_LESS = 0x0201;
        public const int GL_EQUAL = 0x0202;
        public const int GL_LEQUAL = 0x0203;
        public const int GL_GREATER = 0x0204;
        public const int GL_NOTEQUAL = 0x0205;
        public const int GL_GEQUAL = 0x0206;
        public const int GL_ALWAYS = 0x0207;
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
        public const int GL_NONE = 0;
        public const int GL_FRONT_LEFT = 0x0400;
        public const int GL_FRONT_RIGHT = 0x0401;
        public const int GL_BACK_LEFT = 0x0402;
        public const int GL_BACK_RIGHT = 0x0403;
        public const int GL_FRONT = 0x0404;
        public const int GL_BACK = 0x0405;
        public const int GL_LEFT = 0x0406;
        public const int GL_RIGHT = 0x0407;
        public const int GL_FRONT_AND_BACK = 0x0408;
        public const int GL_NO_ERROR = 0;
        public const int GL_INVALID_ENUM = 0x0500;
        public const int GL_INVALID_VALUE = 0x0501;
        public const int GL_INVALID_OPERATION = 0x0502;
        public const int GL_OUT_OF_MEMORY = 0x0505;
        public const int GL_CW = 0x0900;
        public const int GL_CCW = 0x0901;
        public const int GL_POINT_SIZE = 0x0B11;
        public const int GL_POINT_SIZE_RANGE = 0x0B12;
        public const int GL_POINT_SIZE_GRANULARITY = 0x0B13;
        public const int GL_LINE_SMOOTH = 0x0B20;
        public const int GL_LINE_WIDTH = 0x0B21;
        public const int GL_LINE_WIDTH_RANGE = 0x0B22;
        public const int GL_LINE_WIDTH_GRANULARITY = 0x0B23;
        public const int GL_POLYGON_MODE = 0x0B40;
        public const int GL_POLYGON_SMOOTH = 0x0B41;
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
        public const int GL_BLEND_DST = 0x0BE0;
        public const int GL_BLEND_SRC = 0x0BE1;
        public const int GL_BLEND = 0x0BE2;
        public const int GL_LOGIC_OP_MODE = 0x0BF0;
        public const int GL_DRAW_BUFFER = 0x0C01;
        public const int GL_READ_BUFFER = 0x0C02;
        public const int GL_SCISSOR_BOX = 0x0C10;
        public const int GL_SCISSOR_TEST = 0x0C11;
        public const int GL_COLOR_CLEAR_VALUE = 0x0C22;
        public const int GL_COLOR_WRITEMASK = 0x0C23;
        public const int GL_DOUBLEBUFFER = 0x0C32;
        public const int GL_STEREO = 0x0C33;
        public const int GL_LINE_SMOOTH_HINT = 0x0C52;
        public const int GL_POLYGON_SMOOTH_HINT = 0x0C53;
        public const int GL_UNPACK_SWAP_BYTES = 0x0CF0;
        public const int GL_UNPACK_LSB_FIRST = 0x0CF1;
        public const int GL_UNPACK_ROW_LENGTH = 0x0CF2;
        public const int GL_UNPACK_SKIP_ROWS = 0x0CF3;
        public const int GL_UNPACK_SKIP_PIXELS = 0x0CF4;
        public const int GL_UNPACK_ALIGNMENT = 0x0CF5;
        public const int GL_PACK_SWAP_BYTES = 0x0D00;
        public const int GL_PACK_LSB_FIRST = 0x0D01;
        public const int GL_PACK_ROW_LENGTH = 0x0D02;
        public const int GL_PACK_SKIP_ROWS = 0x0D03;
        public const int GL_PACK_SKIP_PIXELS = 0x0D04;
        public const int GL_PACK_ALIGNMENT = 0x0D05;
        public const int GL_MAX_TEXTURE_SIZE = 0x0D33;
        public const int GL_MAX_VIEWPORT_DIMS = 0x0D3A;
        public const int GL_SUBPIXEL_BITS = 0x0D50;
        public const int GL_TEXTURE_1D = 0x0DE0;
        public const int GL_TEXTURE_2D = 0x0DE1;
        public const int GL_TEXTURE_WIDTH = 0x1000;
        public const int GL_TEXTURE_HEIGHT = 0x1001;
        public const int GL_TEXTURE_BORDER_COLOR = 0x1004;
        public const int GL_DONT_CARE = 0x1100;
        public const int GL_FASTEST = 0x1101;
        public const int GL_NICEST = 0x1102;
        public const int GL_BYTE = 0x1400;
        public const int GL_UNSIGNED_BYTE = 0x1401;
        public const int GL_SHORT = 0x1402;
        public const int GL_UNSIGNED_SHORT = 0x1403;
        public const int GL_INT = 0x1404;
        public const int GL_UNSIGNED_INT = 0x1405;
        public const int GL_FLOAT = 0x1406;
        public const int GL_CLEAR = 0x1500;
        public const int GL_AND = 0x1501;
        public const int GL_AND_REVERSE = 0x1502;
        public const int GL_COPY = 0x1503;
        public const int GL_AND_INVERTED = 0x1504;
        public const int GL_NOOP = 0x1505;
        public const int GL_XOR = 0x1506;
        public const int GL_OR = 0x1507;
        public const int GL_NOR = 0x1508;
        public const int GL_EQUIV = 0x1509;
        public const int GL_INVERT = 0x150A;
        public const int GL_OR_REVERSE = 0x150B;
        public const int GL_COPY_INVERTED = 0x150C;
        public const int GL_OR_INVERTED = 0x150D;
        public const int GL_NAND = 0x150E;
        public const int GL_SET = 0x150F;
        public const int GL_TEXTURE = 0x1702;
        public const int GL_COLOR = 0x1800;
        public const int GL_DEPTH = 0x1801;
        public const int GL_STENCIL = 0x1802;
        public const int GL_STENCIL_INDEX = 0x1901;
        public const int GL_DEPTH_COMPONENT = 0x1902;
        public const int GL_RED = 0x1903;
        public const int GL_GREEN = 0x1904;
        public const int GL_BLUE = 0x1905;
        public const int GL_ALPHA = 0x1906;
        public const int GL_RGB = 0x1907;
        public const int GL_RGBA = 0x1908;
        public const int GL_POINT = 0x1B00;
        public const int GL_LINE = 0x1B01;
        public const int GL_FILL = 0x1B02;
        public const int GL_KEEP = 0x1E00;
        public const int GL_REPLACE = 0x1E01;
        public const int GL_INCR = 0x1E02;
        public const int GL_DECR = 0x1E03;
        public const int GL_VENDOR = 0x1F00;
        public const int GL_RENDERER = 0x1F01;
        public const int GL_VERSION = 0x1F02;
        public const int GL_EXTENSIONS = 0x1F03;
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
        public const int GL_COLOR_LOGIC_OP = 0x0BF2;
        public const int GL_POLYGON_OFFSET_UNITS = 0x2A00;
        public const int GL_POLYGON_OFFSET_POINT = 0x2A01;
        public const int GL_POLYGON_OFFSET_LINE = 0x2A02;
        public const int GL_POLYGON_OFFSET_FILL = 0x8037;
        public const int GL_POLYGON_OFFSET_FACTOR = 0x8038;
        public const int GL_TEXTURE_BINDING_1D = 0x8068;
        public const int GL_TEXTURE_BINDING_2D = 0x8069;
        public const int GL_TEXTURE_INTERNAL_FORMAT = 0x1003;
        public const int GL_TEXTURE_RED_SIZE = 0x805C;
        public const int GL_TEXTURE_GREEN_SIZE = 0x805D;
        public const int GL_TEXTURE_BLUE_SIZE = 0x805E;
        public const int GL_TEXTURE_ALPHA_SIZE = 0x805F;
        public const int GL_DOUBLE = 0x140A;
        public const int GL_PROXY_TEXTURE_1D = 0x8063;
        public const int GL_PROXY_TEXTURE_2D = 0x8064;
        public const int GL_R3_G3_B2 = 0x2A10;
        public const int GL_RGB4 = 0x804F;
        public const int GL_RGB5 = 0x8050;
        public const int GL_RGB8 = 0x8051;
        public const int GL_RGB10 = 0x8052;
        public const int GL_RGB12 = 0x8053;
        public const int GL_RGB16 = 0x8054;
        public const int GL_RGBA2 = 0x8055;
        public const int GL_RGBA4 = 0x8056;
        public const int GL_RGB5_A1 = 0x8057;
        public const int GL_RGBA8 = 0x8058;
        public const int GL_RGB10_A2 = 0x8059;
        public const int GL_RGBA12 = 0x805A;
        public const int GL_RGBA16 = 0x805B;
        public const int GL_UNSIGNED_BYTE_3_3_2 = 0x8032;
        public const int GL_UNSIGNED_SHORT_4_4_4_4 = 0x8033;
        public const int GL_UNSIGNED_SHORT_5_5_5_1 = 0x8034;
        public const int GL_UNSIGNED_INT_8_8_8_8 = 0x8035;
        public const int GL_UNSIGNED_INT_10_10_10_2 = 0x8036;
        public const int GL_TEXTURE_BINDING_3D = 0x806A;
        public const int GL_PACK_SKIP_IMAGES = 0x806B;
        public const int GL_PACK_IMAGE_HEIGHT = 0x806C;
        public const int GL_UNPACK_SKIP_IMAGES = 0x806D;
        public const int GL_UNPACK_IMAGE_HEIGHT = 0x806E;
        public const int GL_TEXTURE_3D = 0x806F;
        public const int GL_PROXY_TEXTURE_3D = 0x8070;
        public const int GL_TEXTURE_DEPTH = 0x8071;
        public const int GL_TEXTURE_WRAP_R = 0x8072;
        public const int GL_MAX_3D_TEXTURE_SIZE = 0x8073;
        public const int GL_UNSIGNED_BYTE_2_3_3_REV = 0x8362;
        public const int GL_UNSIGNED_SHORT_5_6_5 = 0x8363;
        public const int GL_UNSIGNED_SHORT_5_6_5_REV = 0x8364;
        public const int GL_UNSIGNED_SHORT_4_4_4_4_REV = 0x8365;
        public const int GL_UNSIGNED_SHORT_1_5_5_5_REV = 0x8366;
        public const int GL_UNSIGNED_INT_8_8_8_8_REV = 0x8367;
        public const int GL_UNSIGNED_INT_2_10_10_10_REV = 0x8368;
        public const int GL_BGR = 0x80E0;
        public const int GL_BGRA = 0x80E1;
        public const int GL_MAX_ELEMENTS_VERTICES = 0x80E8;
        public const int GL_MAX_ELEMENTS_INDICES = 0x80E9;
        public const int GL_CLAMP_TO_EDGE = 0x812F;
        public const int GL_TEXTURE_MIN_LOD = 0x813A;
        public const int GL_TEXTURE_MAX_LOD = 0x813B;
        public const int GL_TEXTURE_BASE_LEVEL = 0x813C;
        public const int GL_TEXTURE_MAX_LEVEL = 0x813D;
        public const int GL_SMOOTH_POINT_SIZE_RANGE = 0x0B12;
        public const int GL_SMOOTH_POINT_SIZE_GRANULARITY = 0x0B13;
        public const int GL_SMOOTH_LINE_WIDTH_RANGE = 0x0B22;
        public const int GL_SMOOTH_LINE_WIDTH_GRANULARITY = 0x0B23;
        public const int GL_ALIASED_LINE_WIDTH_RANGE = 0x846E;
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
        public const int GL_MULTISAMPLE = 0x809D;
        public const int GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E;
        public const int GL_SAMPLE_ALPHA_TO_ONE = 0x809F;
        public const int GL_SAMPLE_COVERAGE = 0x80A0;
        public const int GL_SAMPLE_BUFFERS = 0x80A8;
        public const int GL_SAMPLES = 0x80A9;
        public const int GL_SAMPLE_COVERAGE_VALUE = 0x80AA;
        public const int GL_SAMPLE_COVERAGE_INVERT = 0x80AB;
        public const int GL_TEXTURE_CUBE_MAP = 0x8513;
        public const int GL_TEXTURE_BINDING_CUBE_MAP = 0x8514;
        public const int GL_TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515;
        public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_X = 0x8516;
        public const int GL_TEXTURE_CUBE_MAP_POSITIVE_Y = 0x8517;
        public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_Y = 0x8518;
        public const int GL_TEXTURE_CUBE_MAP_POSITIVE_Z = 0x8519;
        public const int GL_TEXTURE_CUBE_MAP_NEGATIVE_Z = 0x851A;
        public const int GL_PROXY_TEXTURE_CUBE_MAP = 0x851B;
        public const int GL_MAX_CUBE_MAP_TEXTURE_SIZE = 0x851C;
        public const int GL_COMPRESSED_RGB = 0x84ED;
        public const int GL_COMPRESSED_RGBA = 0x84EE;
        public const int GL_TEXTURE_COMPRESSION_HINT = 0x84EF;
        public const int GL_TEXTURE_COMPRESSED_IMAGE_SIZE = 0x86A0;
        public const int GL_TEXTURE_COMPRESSED = 0x86A1;
        public const int GL_NUM_COMPRESSED_TEXTURE_FORMATS = 0x86A2;
        public const int GL_COMPRESSED_TEXTURE_FORMATS = 0x86A3;
        public const int GL_CLAMP_TO_BORDER = 0x812D;
        public const int GL_BLEND_DST_RGB = 0x80C8;
        public const int GL_BLEND_SRC_RGB = 0x80C9;
        public const int GL_BLEND_DST_ALPHA = 0x80CA;
        public const int GL_BLEND_SRC_ALPHA = 0x80CB;
        public const int GL_POINT_FADE_THRESHOLD_SIZE = 0x8128;
        public const int GL_DEPTH_COMPONENT16 = 0x81A5;
        public const int GL_DEPTH_COMPONENT24 = 0x81A6;
        public const int GL_DEPTH_COMPONENT32 = 0x81A7;
        public const int GL_MIRRORED_REPEAT = 0x8370;
        public const int GL_MAX_TEXTURE_LOD_BIAS = 0x84FD;
        public const int GL_TEXTURE_LOD_BIAS = 0x8501;
        public const int GL_INCR_WRAP = 0x8507;
        public const int GL_DECR_WRAP = 0x8508;
        public const int GL_TEXTURE_DEPTH_SIZE = 0x884A;
        public const int GL_TEXTURE_COMPARE_MODE = 0x884C;
        public const int GL_TEXTURE_COMPARE_FUNC = 0x884D;
        public const int GL_BLEND_COLOR = 0x8005;
        public const int GL_BLEND_EQUATION = 0x8009;
        public const int GL_CONSTANT_COLOR = 0x8001;
        public const int GL_ONE_MINUS_CONSTANT_COLOR = 0x8002;
        public const int GL_CONSTANT_ALPHA = 0x8003;
        public const int GL_ONE_MINUS_CONSTANT_ALPHA = 0x8004;
        public const int GL_FUNC_ADD = 0x8006;
        public const int GL_FUNC_REVERSE_SUBTRACT = 0x800B;
        public const int GL_FUNC_SUBTRACT = 0x800A;
        public const int GL_MIN = 0x8007;
        public const int GL_MAX = 0x8008;
        public const int GL_BUFFER_SIZE = 0x8764;
        public const int GL_BUFFER_USAGE = 0x8765;
        public const int GL_QUERY_COUNTER_BITS = 0x8864;
        public const int GL_CURRENT_QUERY = 0x8865;
        public const int GL_QUERY_RESULT = 0x8866;
        public const int GL_QUERY_RESULT_AVAILABLE = 0x8867;
        public const int GL_ARRAY_BUFFER = 0x8892;
        public const int GL_ELEMENT_ARRAY_BUFFER = 0x8893;
        public const int GL_ARRAY_BUFFER_BINDING = 0x8894;
        public const int GL_ELEMENT_ARRAY_BUFFER_BINDING = 0x8895;
        public const int GL_VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 0x889F;
        public const int GL_READ_ONLY = 0x88B8;
        public const int GL_WRITE_ONLY = 0x88B9;
        public const int GL_READ_WRITE = 0x88BA;
        public const int GL_BUFFER_ACCESS = 0x88BB;
        public const int GL_BUFFER_MAPPED = 0x88BC;
        public const int GL_BUFFER_MAP_POINTER = 0x88BD;
        public const int GL_STREAM_DRAW = 0x88E0;
        public const int GL_STREAM_READ = 0x88E1;
        public const int GL_STREAM_COPY = 0x88E2;
        public const int GL_STATIC_DRAW = 0x88E4;
        public const int GL_STATIC_READ = 0x88E5;
        public const int GL_STATIC_COPY = 0x88E6;
        public const int GL_DYNAMIC_DRAW = 0x88E8;
        public const int GL_DYNAMIC_READ = 0x88E9;
        public const int GL_DYNAMIC_COPY = 0x88EA;
        public const int GL_SAMPLES_PASSED = 0x8914;
        public const int GL_SRC1_ALPHA = 0x8589;
        public const int GL_BLEND_EQUATION_RGB = 0x8009;
        public const int GL_VERTEX_ATTRIB_ARRAY_ENABLED = 0x8622;
        public const int GL_VERTEX_ATTRIB_ARRAY_SIZE = 0x8623;
        public const int GL_VERTEX_ATTRIB_ARRAY_STRIDE = 0x8624;
        public const int GL_VERTEX_ATTRIB_ARRAY_TYPE = 0x8625;
        public const int GL_CURRENT_VERTEX_ATTRIB = 0x8626;
        public const int GL_VERTEX_PROGRAM_POINT_SIZE = 0x8642;
        public const int GL_VERTEX_ATTRIB_ARRAY_POINTER = 0x8645;
        public const int GL_STENCIL_BACK_FUNC = 0x8800;
        public const int GL_STENCIL_BACK_FAIL = 0x8801;
        public const int GL_STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802;
        public const int GL_STENCIL_BACK_PASS_DEPTH_PASS = 0x8803;
        public const int GL_MAX_DRAW_BUFFERS = 0x8824;
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
        public const int GL_BLEND_EQUATION_ALPHA = 0x883D;
        public const int GL_MAX_VERTEX_ATTRIBS = 0x8869;
        public const int GL_VERTEX_ATTRIB_ARRAY_NORMALIZED = 0x886A;
        public const int GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872;
        public const int GL_FRAGMENT_SHADER = 0x8B30;
        public const int GL_VERTEX_SHADER = 0x8B31;
        public const int GL_MAX_FRAGMENT_UNIFORM_COMPONENTS = 0x8B49;
        public const int GL_MAX_VERTEX_UNIFORM_COMPONENTS = 0x8B4A;
        public const int GL_MAX_VARYING_FLOATS = 0x8B4B;
        public const int GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C;
        public const int GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D;
        public const int GL_SHADER_TYPE = 0x8B4F;
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
        public const int GL_SAMPLER_1D = 0x8B5D;
        public const int GL_SAMPLER_2D = 0x8B5E;
        public const int GL_SAMPLER_3D = 0x8B5F;
        public const int GL_SAMPLER_CUBE = 0x8B60;
        public const int GL_SAMPLER_1D_SHADOW = 0x8B61;
        public const int GL_SAMPLER_2D_SHADOW = 0x8B62;
        public const int GL_DELETE_STATUS = 0x8B80;
        public const int GL_COMPILE_STATUS = 0x8B81;
        public const int GL_LINK_STATUS = 0x8B82;
        public const int GL_VALIDATE_STATUS = 0x8B83;
        public const int GL_INFO_LOG_LENGTH = 0x8B84;
        public const int GL_ATTACHED_SHADERS = 0x8B85;
        public const int GL_ACTIVE_UNIFORMS = 0x8B86;
        public const int GL_ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87;
        public const int GL_SHADER_SOURCE_LENGTH = 0x8B88;
        public const int GL_ACTIVE_ATTRIBUTES = 0x8B89;
        public const int GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A;
        public const int GL_FRAGMENT_SHADER_DERIVATIVE_HINT = 0x8B8B;
        public const int GL_SHADING_LANGUAGE_VERSION = 0x8B8C;
        public const int GL_CURRENT_PROGRAM = 0x8B8D;
        public const int GL_POINT_SPRITE_COORD_ORIGIN = 0x8CA0;
        public const int GL_LOWER_LEFT = 0x8CA1;
        public const int GL_UPPER_LEFT = 0x8CA2;
        public const int GL_STENCIL_BACK_REF = 0x8CA3;
        public const int GL_STENCIL_BACK_VALUE_MASK = 0x8CA4;
        public const int GL_STENCIL_BACK_WRITEMASK = 0x8CA5;
        public const int GL_PIXEL_PACK_BUFFER = 0x88EB;
        public const int GL_PIXEL_UNPACK_BUFFER = 0x88EC;
        public const int GL_PIXEL_PACK_BUFFER_BINDING = 0x88ED;
        public const int GL_PIXEL_UNPACK_BUFFER_BINDING = 0x88EF;
        public const int GL_FLOAT_MAT2x3 = 0x8B65;
        public const int GL_FLOAT_MAT2x4 = 0x8B66;
        public const int GL_FLOAT_MAT3x2 = 0x8B67;
        public const int GL_FLOAT_MAT3x4 = 0x8B68;
        public const int GL_FLOAT_MAT4x2 = 0x8B69;
        public const int GL_FLOAT_MAT4x3 = 0x8B6A;
        public const int GL_SRGB = 0x8C40;
        public const int GL_SRGB8 = 0x8C41;
        public const int GL_SRGB_ALPHA = 0x8C42;
        public const int GL_SRGB8_ALPHA8 = 0x8C43;
        public const int GL_COMPRESSED_SRGB = 0x8C48;
        public const int GL_COMPRESSED_SRGB_ALPHA = 0x8C49;
        public const int GL_COMPARE_REF_TO_TEXTURE = 0x884E;
        public const int GL_CLIP_DISTANCE0 = 0x3000;
        public const int GL_CLIP_DISTANCE1 = 0x3001;
        public const int GL_CLIP_DISTANCE2 = 0x3002;
        public const int GL_CLIP_DISTANCE3 = 0x3003;
        public const int GL_CLIP_DISTANCE4 = 0x3004;
        public const int GL_CLIP_DISTANCE5 = 0x3005;
        public const int GL_CLIP_DISTANCE6 = 0x3006;
        public const int GL_CLIP_DISTANCE7 = 0x3007;
        public const int GL_MAX_CLIP_DISTANCES = 0x0D32;
        public const int GL_MAJOR_VERSION = 0x821B;
        public const int GL_MINOR_VERSION = 0x821C;
        public const int GL_NUM_EXTENSIONS = 0x821D;
        public const int GL_CONTEXT_FLAGS = 0x821E;
        public const int GL_COMPRESSED_RED = 0x8225;
        public const int GL_COMPRESSED_RG = 0x8226;
        public const int GL_CONTEXT_FLAG_FORWARD_COMPATIBLE_BIT = 0x00000001;
        public const int GL_RGBA32F = 0x8814;
        public const int GL_RGB32F = 0x8815;
        public const int GL_RGBA16F = 0x881A;
        public const int GL_RGB16F = 0x881B;
        public const int GL_VERTEX_ATTRIB_ARRAY_INTEGER = 0x88FD;
        public const int GL_MAX_ARRAY_TEXTURE_LAYERS = 0x88FF;
        public const int GL_MIN_PROGRAM_TEXEL_OFFSET = 0x8904;
        public const int GL_MAX_PROGRAM_TEXEL_OFFSET = 0x8905;
        public const int GL_CLAMP_READ_COLOR = 0x891C;
        public const int GL_FIXED_ONLY = 0x891D;
        public const int GL_MAX_VARYING_COMPONENTS = 0x8B4B;
        public const int GL_TEXTURE_1D_ARRAY = 0x8C18;
        public const int GL_PROXY_TEXTURE_1D_ARRAY = 0x8C19;
        public const int GL_TEXTURE_2D_ARRAY = 0x8C1A;
        public const int GL_PROXY_TEXTURE_2D_ARRAY = 0x8C1B;
        public const int GL_TEXTURE_BINDING_1D_ARRAY = 0x8C1C;
        public const int GL_TEXTURE_BINDING_2D_ARRAY = 0x8C1D;
        public const int GL_R11F_G11F_B10F = 0x8C3A;
        public const int GL_UNSIGNED_INT_10F_11F_11F_REV = 0x8C3B;
        public const int GL_RGB9_E5 = 0x8C3D;
        public const int GL_UNSIGNED_INT_5_9_9_9_REV = 0x8C3E;
        public const int GL_TEXTURE_SHARED_SIZE = 0x8C3F;
        public const int GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 0x8C76;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_MODE = 0x8C7F;
        public const int GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 0x8C80;
        public const int GL_TRANSFORM_FEEDBACK_VARYINGS = 0x8C83;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_START = 0x8C84;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_SIZE = 0x8C85;
        public const int GL_PRIMITIVES_GENERATED = 0x8C87;
        public const int GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 0x8C88;
        public const int GL_RASTERIZER_DISCARD = 0x8C89;
        public const int GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 0x8C8A;
        public const int GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 0x8C8B;
        public const int GL_INTERLEAVED_ATTRIBS = 0x8C8C;
        public const int GL_SEPARATE_ATTRIBS = 0x8C8D;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER = 0x8C8E;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_BINDING = 0x8C8F;
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
        public const int GL_RED_INTEGER = 0x8D94;
        public const int GL_GREEN_INTEGER = 0x8D95;
        public const int GL_BLUE_INTEGER = 0x8D96;
        public const int GL_RGB_INTEGER = 0x8D98;
        public const int GL_RGBA_INTEGER = 0x8D99;
        public const int GL_BGR_INTEGER = 0x8D9A;
        public const int GL_BGRA_INTEGER = 0x8D9B;
        public const int GL_SAMPLER_1D_ARRAY = 0x8DC0;
        public const int GL_SAMPLER_2D_ARRAY = 0x8DC1;
        public const int GL_SAMPLER_1D_ARRAY_SHADOW = 0x8DC3;
        public const int GL_SAMPLER_2D_ARRAY_SHADOW = 0x8DC4;
        public const int GL_SAMPLER_CUBE_SHADOW = 0x8DC5;
        public const int GL_UNSIGNED_INT_VEC2 = 0x8DC6;
        public const int GL_UNSIGNED_INT_VEC3 = 0x8DC7;
        public const int GL_UNSIGNED_INT_VEC4 = 0x8DC8;
        public const int GL_INT_SAMPLER_1D = 0x8DC9;
        public const int GL_INT_SAMPLER_2D = 0x8DCA;
        public const int GL_INT_SAMPLER_3D = 0x8DCB;
        public const int GL_INT_SAMPLER_CUBE = 0x8DCC;
        public const int GL_INT_SAMPLER_1D_ARRAY = 0x8DCE;
        public const int GL_INT_SAMPLER_2D_ARRAY = 0x8DCF;
        public const int GL_UNSIGNED_INT_SAMPLER_1D = 0x8DD1;
        public const int GL_UNSIGNED_INT_SAMPLER_2D = 0x8DD2;
        public const int GL_UNSIGNED_INT_SAMPLER_3D = 0x8DD3;
        public const int GL_UNSIGNED_INT_SAMPLER_CUBE = 0x8DD4;
        public const int GL_UNSIGNED_INT_SAMPLER_1D_ARRAY = 0x8DD6;
        public const int GL_UNSIGNED_INT_SAMPLER_2D_ARRAY = 0x8DD7;
        public const int GL_QUERY_WAIT = 0x8E13;
        public const int GL_QUERY_NO_WAIT = 0x8E14;
        public const int GL_QUERY_BY_REGION_WAIT = 0x8E15;
        public const int GL_QUERY_BY_REGION_NO_WAIT = 0x8E16;
        public const int GL_BUFFER_ACCESS_FLAGS = 0x911F;
        public const int GL_BUFFER_MAP_LENGTH = 0x9120;
        public const int GL_BUFFER_MAP_OFFSET = 0x9121;
        public const int GL_DEPTH_COMPONENT32F = 0x8CAC;
        public const int GL_DEPTH32F_STENCIL8 = 0x8CAD;
        public const int GL_FLOAT_32_UNSIGNED_INT_24_8_REV = 0x8DAD;
        public const int GL_INVALID_FRAMEBUFFER_OPERATION = 0x0506;
        public const int GL_FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING = 0x8210;
        public const int GL_FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE = 0x8211;
        public const int GL_FRAMEBUFFER_ATTACHMENT_RED_SIZE = 0x8212;
        public const int GL_FRAMEBUFFER_ATTACHMENT_GREEN_SIZE = 0x8213;
        public const int GL_FRAMEBUFFER_ATTACHMENT_BLUE_SIZE = 0x8214;
        public const int GL_FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE = 0x8215;
        public const int GL_FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE = 0x8216;
        public const int GL_FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE = 0x8217;
        public const int GL_FRAMEBUFFER_DEFAULT = 0x8218;
        public const int GL_FRAMEBUFFER_UNDEFINED = 0x8219;
        public const int GL_DEPTH_STENCIL_ATTACHMENT = 0x821A;
        public const int GL_MAX_RENDERBUFFER_SIZE = 0x84E8;
        public const int GL_DEPTH_STENCIL = 0x84F9;
        public const int GL_UNSIGNED_INT_24_8 = 0x84FA;
        public const int GL_DEPTH24_STENCIL8 = 0x88F0;
        public const int GL_TEXTURE_STENCIL_SIZE = 0x88F1;
        public const int GL_TEXTURE_RED_TYPE = 0x8C10;
        public const int GL_TEXTURE_GREEN_TYPE = 0x8C11;
        public const int GL_TEXTURE_BLUE_TYPE = 0x8C12;
        public const int GL_TEXTURE_ALPHA_TYPE = 0x8C13;
        public const int GL_TEXTURE_DEPTH_TYPE = 0x8C16;
        public const int GL_UNSIGNED_NORMALIZED = 0x8C17;
        public const int GL_FRAMEBUFFER_BINDING = 0x8CA6;
        public const int GL_DRAW_FRAMEBUFFER_BINDING = 0x8CA6;
        public const int GL_RENDERBUFFER_BINDING = 0x8CA7;
        public const int GL_READ_FRAMEBUFFER = 0x8CA8;
        public const int GL_DRAW_FRAMEBUFFER = 0x8CA9;
        public const int GL_READ_FRAMEBUFFER_BINDING = 0x8CAA;
        public const int GL_RENDERBUFFER_SAMPLES = 0x8CAB;
        public const int GL_FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE = 0x8CD0;
        public const int GL_FRAMEBUFFER_ATTACHMENT_OBJECT_NAME = 0x8CD1;
        public const int GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL = 0x8CD2;
        public const int GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE = 0x8CD3;
        public const int GL_FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER = 0x8CD4;
        public const int GL_FRAMEBUFFER_COMPLETE = 0x8CD5;
        public const int GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 0x8CD6;
        public const int GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7;
        public const int GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER = 0x8CDB;
        public const int GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER = 0x8CDC;
        public const int GL_FRAMEBUFFER_UNSUPPORTED = 0x8CDD;
        public const int GL_MAX_COLOR_ATTACHMENTS = 0x8CDF;
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
        public const int GL_COLOR_ATTACHMENT16 = 0x8CF0;
        public const int GL_COLOR_ATTACHMENT17 = 0x8CF1;
        public const int GL_COLOR_ATTACHMENT18 = 0x8CF2;
        public const int GL_COLOR_ATTACHMENT19 = 0x8CF3;
        public const int GL_COLOR_ATTACHMENT20 = 0x8CF4;
        public const int GL_COLOR_ATTACHMENT21 = 0x8CF5;
        public const int GL_COLOR_ATTACHMENT22 = 0x8CF6;
        public const int GL_COLOR_ATTACHMENT23 = 0x8CF7;
        public const int GL_COLOR_ATTACHMENT24 = 0x8CF8;
        public const int GL_COLOR_ATTACHMENT25 = 0x8CF9;
        public const int GL_COLOR_ATTACHMENT26 = 0x8CFA;
        public const int GL_COLOR_ATTACHMENT27 = 0x8CFB;
        public const int GL_COLOR_ATTACHMENT28 = 0x8CFC;
        public const int GL_COLOR_ATTACHMENT29 = 0x8CFD;
        public const int GL_COLOR_ATTACHMENT30 = 0x8CFE;
        public const int GL_COLOR_ATTACHMENT31 = 0x8CFF;
        public const int GL_DEPTH_ATTACHMENT = 0x8D00;
        public const int GL_STENCIL_ATTACHMENT = 0x8D20;
        public const int GL_FRAMEBUFFER = 0x8D40;
        public const int GL_RENDERBUFFER = 0x8D41;
        public const int GL_RENDERBUFFER_WIDTH = 0x8D42;
        public const int GL_RENDERBUFFER_HEIGHT = 0x8D43;
        public const int GL_RENDERBUFFER_INTERNAL_FORMAT = 0x8D44;
        public const int GL_STENCIL_INDEX1 = 0x8D46;
        public const int GL_STENCIL_INDEX4 = 0x8D47;
        public const int GL_STENCIL_INDEX8 = 0x8D48;
        public const int GL_STENCIL_INDEX16 = 0x8D49;
        public const int GL_RENDERBUFFER_RED_SIZE = 0x8D50;
        public const int GL_RENDERBUFFER_GREEN_SIZE = 0x8D51;
        public const int GL_RENDERBUFFER_BLUE_SIZE = 0x8D52;
        public const int GL_RENDERBUFFER_ALPHA_SIZE = 0x8D53;
        public const int GL_RENDERBUFFER_DEPTH_SIZE = 0x8D54;
        public const int GL_RENDERBUFFER_STENCIL_SIZE = 0x8D55;
        public const int GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE = 0x8D56;
        public const int GL_MAX_SAMPLES = 0x8D57;
        public const int GL_FRAMEBUFFER_SRGB = 0x8DB9;
        public const int GL_HALF_FLOAT = 0x140B;
        public const int GL_MAP_READ_BIT = 0x0001;
        public const int GL_MAP_WRITE_BIT = 0x0002;
        public const int GL_MAP_INVALIDATE_RANGE_BIT = 0x0004;
        public const int GL_MAP_INVALIDATE_BUFFER_BIT = 0x0008;
        public const int GL_MAP_FLUSH_EXPLICIT_BIT = 0x0010;
        public const int GL_MAP_UNSYNCHRONIZED_BIT = 0x0020;
        public const int GL_COMPRESSED_RED_RGTC1 = 0x8DBB;
        public const int GL_COMPRESSED_SIGNED_RED_RGTC1 = 0x8DBC;
        public const int GL_COMPRESSED_RG_RGTC2 = 0x8DBD;
        public const int GL_COMPRESSED_SIGNED_RG_RGTC2 = 0x8DBE;
        public const int GL_RG = 0x8227;
        public const int GL_RG_INTEGER = 0x8228;
        public const int GL_R8 = 0x8229;
        public const int GL_R16 = 0x822A;
        public const int GL_RG8 = 0x822B;
        public const int GL_RG16 = 0x822C;
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
        public const int GL_VERTEX_ARRAY_BINDING = 0x85B5;
        public const int GL_SAMPLER_2D_RECT = 0x8B63;
        public const int GL_SAMPLER_2D_RECT_SHADOW = 0x8B64;
        public const int GL_SAMPLER_BUFFER = 0x8DC2;
        public const int GL_INT_SAMPLER_2D_RECT = 0x8DCD;
        public const int GL_INT_SAMPLER_BUFFER = 0x8DD0;
        public const int GL_UNSIGNED_INT_SAMPLER_2D_RECT = 0x8DD5;
        public const int GL_UNSIGNED_INT_SAMPLER_BUFFER = 0x8DD8;
        public const int GL_TEXTURE_BUFFER = 0x8C2A;
        public const int GL_MAX_TEXTURE_BUFFER_SIZE = 0x8C2B;
        public const int GL_TEXTURE_BINDING_BUFFER = 0x8C2C;
        public const int GL_TEXTURE_BUFFER_DATA_STORE_BINDING = 0x8C2D;
        public const int GL_TEXTURE_RECTANGLE = 0x84F5;
        public const int GL_TEXTURE_BINDING_RECTANGLE = 0x84F6;
        public const int GL_PROXY_TEXTURE_RECTANGLE = 0x84F7;
        public const int GL_MAX_RECTANGLE_TEXTURE_SIZE = 0x84F8;
        public const int GL_R8_SNORM = 0x8F94;
        public const int GL_RG8_SNORM = 0x8F95;
        public const int GL_RGB8_SNORM = 0x8F96;
        public const int GL_RGBA8_SNORM = 0x8F97;
        public const int GL_R16_SNORM = 0x8F98;
        public const int GL_RG16_SNORM = 0x8F99;
        public const int GL_RGB16_SNORM = 0x8F9A;
        public const int GL_RGBA16_SNORM = 0x8F9B;
        public const int GL_SIGNED_NORMALIZED = 0x8F9C;
        public const int GL_PRIMITIVE_RESTART = 0x8F9D;
        public const int GL_PRIMITIVE_RESTART_INDEX = 0x8F9E;
        public const int GL_COPY_READ_BUFFER = 0x8F36;
        public const int GL_COPY_WRITE_BUFFER = 0x8F37;
        public const int GL_UNIFORM_BUFFER = 0x8A11;
        public const int GL_UNIFORM_BUFFER_BINDING = 0x8A28;
        public const int GL_UNIFORM_BUFFER_START = 0x8A29;
        public const int GL_UNIFORM_BUFFER_SIZE = 0x8A2A;
        public const int GL_MAX_VERTEX_UNIFORM_BLOCKS = 0x8A2B;
        public const int GL_MAX_GEOMETRY_UNIFORM_BLOCKS = 0x8A2C;
        public const int GL_MAX_FRAGMENT_UNIFORM_BLOCKS = 0x8A2D;
        public const int GL_MAX_COMBINED_UNIFORM_BLOCKS = 0x8A2E;
        public const int GL_MAX_UNIFORM_BUFFER_BINDINGS = 0x8A2F;
        public const int GL_MAX_UNIFORM_BLOCK_SIZE = 0x8A30;
        public const int GL_MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS = 0x8A31;
        public const int GL_MAX_COMBINED_GEOMETRY_UNIFORM_COMPONENTS = 0x8A32;
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
        public const int GL_UNIFORM_BLOCK_REFERENCED_BY_GEOMETRY_SHADER = 0x8A45;
        public const int GL_UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER = 0x8A46;
        public const uint GL_INVALID_INDEX = 0xFFFFFFFF;
        public const int GL_CONTEXT_CORE_PROFILE_BIT = 0x00000001;
        public const int GL_CONTEXT_COMPATIBILITY_PROFILE_BIT = 0x00000002;
        public const int GL_LINES_ADJACENCY = 0x000A;
        public const int GL_LINE_STRIP_ADJACENCY = 0x000B;
        public const int GL_TRIANGLES_ADJACENCY = 0x000C;
        public const int GL_TRIANGLE_STRIP_ADJACENCY = 0x000D;
        public const int GL_PROGRAM_POINT_SIZE = 0x8642;
        public const int GL_MAX_GEOMETRY_TEXTURE_IMAGE_UNITS = 0x8C29;
        public const int GL_FRAMEBUFFER_ATTACHMENT_LAYERED = 0x8DA7;
        public const int GL_FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS = 0x8DA8;
        public const int GL_GEOMETRY_SHADER = 0x8DD9;
        public const int GL_GEOMETRY_VERTICES_OUT = 0x8916;
        public const int GL_GEOMETRY_INPUT_TYPE = 0x8917;
        public const int GL_GEOMETRY_OUTPUT_TYPE = 0x8918;
        public const int GL_MAX_GEOMETRY_UNIFORM_COMPONENTS = 0x8DDF;
        public const int GL_MAX_GEOMETRY_OUTPUT_VERTICES = 0x8DE0;
        public const int GL_MAX_GEOMETRY_TOTAL_OUTPUT_COMPONENTS = 0x8DE1;
        public const int GL_MAX_VERTEX_OUTPUT_COMPONENTS = 0x9122;
        public const int GL_MAX_GEOMETRY_INPUT_COMPONENTS = 0x9123;
        public const int GL_MAX_GEOMETRY_OUTPUT_COMPONENTS = 0x9124;
        public const int GL_MAX_FRAGMENT_INPUT_COMPONENTS = 0x9125;
        public const int GL_CONTEXT_PROFILE_MASK = 0x9126;
        public const int GL_DEPTH_CLAMP = 0x864F;
        public const int GL_QUADS_FOLLOW_PROVOKING_VERTEX_CONVENTION = 0x8E4C;
        public const int GL_FIRST_VERTEX_CONVENTION = 0x8E4D;
        public const int GL_LAST_VERTEX_CONVENTION = 0x8E4E;
        public const int GL_PROVOKING_VERTEX = 0x8E4F;
        public const int GL_TEXTURE_CUBE_MAP_SEAMLESS = 0x884F;
        public const int GL_MAX_SERVER_WAIT_TIMEOUT = 0x9111;
        public const int GL_OBJECT_TYPE = 0x9112;
        public const int GL_SYNC_CONDITION = 0x9113;
        public const int GL_SYNC_STATUS = 0x9114;
        public const int GL_SYNC_FLAGS = 0x9115;
        public const int GL_SYNC_FENCE = 0x9116;
        public const int GL_SYNC_GPU_COMMANDS_COMPLETE = 0x9117;
        public const int GL_UNSIGNALED = 0x9118;
        public const int GL_SIGNALED = 0x9119;
        public const int GL_ALREADY_SIGNALED = 0x911A;
        public const int GL_TIMEOUT_EXPIRED = 0x911B;
        public const int GL_CONDITION_SATISFIED = 0x911C;
        public const int GL_WAIT_FAILED = 0x911D;
        public const ulong GL_TIMEOUT_IGNORED = 0xFFFFFFFFFFFFFFFF;
        public const int GL_SYNC_FLUSH_COMMANDS_BIT = 0x00000001;
        public const int GL_SAMPLE_POSITION = 0x8E50;
        public const int GL_SAMPLE_MASK = 0x8E51;
        public const int GL_SAMPLE_MASK_VALUE = 0x8E52;
        public const int GL_MAX_SAMPLE_MASK_WORDS = 0x8E59;
        public const int GL_TEXTURE_2D_MULTISAMPLE = 0x9100;
        public const int GL_PROXY_TEXTURE_2D_MULTISAMPLE = 0x9101;
        public const int GL_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9102;
        public const int GL_PROXY_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9103;
        public const int GL_TEXTURE_BINDING_2D_MULTISAMPLE = 0x9104;
        public const int GL_TEXTURE_BINDING_2D_MULTISAMPLE_ARRAY = 0x9105;
        public const int GL_TEXTURE_SAMPLES = 0x9106;
        public const int GL_TEXTURE_FIXED_SAMPLE_LOCATIONS = 0x9107;
        public const int GL_SAMPLER_2D_MULTISAMPLE = 0x9108;
        public const int GL_INT_SAMPLER_2D_MULTISAMPLE = 0x9109;
        public const int GL_UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE = 0x910A;
        public const int GL_SAMPLER_2D_MULTISAMPLE_ARRAY = 0x910B;
        public const int GL_INT_SAMPLER_2D_MULTISAMPLE_ARRAY = 0x910C;
        public const int GL_UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE_ARRAY = 0x910D;
        public const int GL_MAX_COLOR_TEXTURE_SAMPLES = 0x910E;
        public const int GL_MAX_DEPTH_TEXTURE_SAMPLES = 0x910F;
        public const int GL_MAX_INTEGER_SAMPLES = 0x9110;
        public const int GL_VERTEX_ATTRIB_ARRAY_DIVISOR = 0x88FE;
        public const int GL_SRC1_COLOR = 0x88F9;
        public const int GL_ONE_MINUS_SRC1_COLOR = 0x88FA;
        public const int GL_ONE_MINUS_SRC1_ALPHA = 0x88FB;
        public const int GL_MAX_DUAL_SOURCE_DRAW_BUFFERS = 0x88FC;
        public const int GL_ANY_SAMPLES_PASSED = 0x8C2F;
        public const int GL_SAMPLER_BINDING = 0x8919;
        public const int GL_RGB10_A2UI = 0x906F;
        public const int GL_TEXTURE_SWIZZLE_R = 0x8E42;
        public const int GL_TEXTURE_SWIZZLE_G = 0x8E43;
        public const int GL_TEXTURE_SWIZZLE_B = 0x8E44;
        public const int GL_TEXTURE_SWIZZLE_A = 0x8E45;
        public const int GL_TEXTURE_SWIZZLE_RGBA = 0x8E46;
        public const int GL_TIME_ELAPSED = 0x88BF;
        public const int GL_TIMESTAMP = 0x8E28;
        public const int GL_INT_2_10_10_10_REV = 0x8D9F;
        #endregion
    }
}
