using System;
using System.Runtime.InteropServices;

namespace OpenGL.ES
{
    public static unsafe class GLES30
    {
#if ANDROID
    private const string LIB = "GLESv2";
#elif IOS || __IOS__
    private const string LIB = "__Internal";
#else
    private const string LIB = "GLESv2";
#endif

        [DllImport(LIB, EntryPoint = "glActiveTexture")]
        public static extern void glActiveTexture(int texture);

        [DllImport(LIB, EntryPoint = "glAttachShader")]
        public static extern void glAttachShader(int program, int shader);

        [DllImport(LIB, EntryPoint = "glBindAttribLocation")]
        public static extern void glBindAttribLocation(int program, int index, string name);

        [DllImport(LIB, EntryPoint = "glBindBuffer")]
        public static extern void glBindBuffer(int target, uint buffer);

        [DllImport(LIB, EntryPoint = "glBindFramebuffer")]
        public static extern void glBindFramebuffer(int target, int framebuffer);

        [DllImport(LIB, EntryPoint = "glBindRenderbuffer")]
        public static extern void glBindRenderbuffer(int target, int renderbuffer);

        [DllImport(LIB, EntryPoint = "glBindTexture")]
        public static extern void glBindTexture(int target, int texture);

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
        public static extern void glCompileShader(int shader);

        [DllImport(LIB, EntryPoint = "glCompressedTexImage2D")]
        public static extern void glCompressedTexImage2D(int target, int level, int internalformat, int width, int height, int border, int imageSize, void* data);

        [DllImport(LIB, EntryPoint = "glCompressedTexSubImage2D")]
        public static extern void glCompressedTexSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int imageSize, void* data);

        [DllImport(LIB, EntryPoint = "glCopyTexImage2D")]
        public static extern void glCopyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height, int border);

        [DllImport(LIB, EntryPoint = "glCopyTexSubImage2D")]
        public static extern void glCopyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width, int height);

        [DllImport(LIB, EntryPoint = "glCreateProgram")]
        public static extern int glCreateProgram();

        [DllImport(LIB, EntryPoint = "glCreateShader")]
        public static extern int glCreateShader(int type);

        [DllImport(LIB, EntryPoint = "glCullFace")]
        public static extern void glCullFace(int mode);

        [DllImport(LIB, EntryPoint = "glDeleteBuffers")]
        public static extern void glDeleteBuffers(int n, uint* buffers);

        public static void glDeleteBuffer(uint buffer)
        {
            glDeleteBuffers(1, &buffer);
        }

        [DllImport(LIB, EntryPoint = "glDeleteFramebuffers")]
        public static extern void glDeleteFramebuffers(int n, int* framebuffers);

        [DllImport(LIB, EntryPoint = "glDeleteProgram")]
        public static extern void glDeleteProgram(int program);

        [DllImport(LIB, EntryPoint = "glDeleteRenderbuffers")]
        public static extern void glDeleteRenderbuffers(int n, int* renderbuffers);

        [DllImport(LIB, EntryPoint = "glDeleteShader")]
        public static extern void glDeleteShader(int shader);

        [DllImport(LIB, EntryPoint = "glDeleteTextures")]
        public static extern void glDeleteTextures(int n, int* textures);

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
        public static extern void glEnableVertexAttribArray(int index);

        [DllImport(LIB, EntryPoint = "glFinish")]
        public static extern void glFinish();

        [DllImport(LIB, EntryPoint = "glFlush")]
        public static extern void glFlush();

        [DllImport(LIB, EntryPoint = "glFramebufferRenderbuffer")]
        public static extern void glFramebufferRenderbuffer(int target, int attachment, int renderbuffertarget, int renderbuffer);

        [DllImport(LIB, EntryPoint = "glFramebufferTexture2D")]
        public static extern void glFramebufferTexture2D(int target, int attachment, int textarget, int texture, int level);

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
        public static extern void glGenFramebuffers(int n, int* framebuffers);

        [DllImport(LIB, EntryPoint = "glGenRenderbuffers")]
        public static extern void glGenRenderbuffers(int n, int* renderbuffers);

        [DllImport(LIB, EntryPoint = "glGenTextures")]
        public static extern void glGenTextures(int n, int* textures);

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
        public static extern void glGetProgramiv(int program, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetProgramInfoLog")]
        public static extern void glGetProgramInfoLog(int program, int bufSize, int* length, byte* infoLog);

        [DllImport(LIB, EntryPoint = "glGetRenderbufferParameteriv")]
        public static extern void glGetRenderbufferParameteriv(int target, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetShaderiv")]
        public static extern void glGetShaderiv(int shader, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetShaderInfoLog")]
        public static extern void glGetShaderInfoLog(int shader, int bufSize, int* length, byte* infoLog);

        [DllImport(LIB, EntryPoint = "glGetShaderPrecisionFormat")]
        public static extern void glGetShaderPrecisionFormat(int shadertype, int precisiontype, int* range, int* precision);

        [DllImport(LIB, EntryPoint = "glGetShaderSource")]
        public static extern void glGetShaderSource(int shader, int bufSize, int* length, byte* source);

        [DllImport(LIB, EntryPoint = "glGetString")]
        public static extern IntPtr glGetString(int name);

        [DllImport(LIB, EntryPoint = "glGetTexParameterfv")]
        public static extern void glGetTexParameterfv(int target, int pname, float* @params);

        [DllImport(LIB, EntryPoint = "glGetTexParameteriv")]
        public static extern void glGetTexParameteriv(int target, int pname, int* @params);

        [DllImport(LIB, EntryPoint = "glGetUniformfv")]
        public static extern void glGetUniformfv(int program, int location, float* @params);

        [DllImport(LIB, EntryPoint = "glGetUniformiv")]
        public static extern void glGetUniformiv(int program, int location, int* @params);

        [DllImport(LIB, EntryPoint = "glGetUniformLocation")]
        public static extern int glGetUniformLocation(int program, string name);

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
        public static extern void glLinkProgram(int program);

        [DllImport(LIB, EntryPoint = "glPixelStorei")]
        public static extern void glPixelStorei(int pname, int param);

        [DllImport(LIB, EntryPoint = "glPolygonOffset")]
        public static extern void glPolygonOffset(float factor, float units);

        [DllImport(LIB, EntryPoint = "glReadPixels")]
        public static extern void glReadPixels(int x, int y, int width, int height, int format, int type, void* pixels);

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
        public static extern void glShaderSource(int shader, int count, string[] source, int* length);

        [DllImport(LIB, EntryPoint = "glStencilFunc")]
        public static extern void glStencilFunc(int func, int @ref, int mask);

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
        public static extern void glUseProgram(int program);

        [DllImport(LIB, EntryPoint = "glValidateProgram")]
        public static extern void glValidateProgram(int program);

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
        public static extern void glVertexAttribPointer(int index, int size, int type, bool normalized, int stride, void* pointer);

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
        public static extern void glBlitFramebuffer(int srcX0, int srcY0, int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, int mask, int filter);

        [DllImport(LIB, EntryPoint = "glRenderbufferStorageMultisample")]
        public static extern void glRenderbufferStorageMultisample(int target, int samples, int internalformat, int width, int height);

        [DllImport(LIB, EntryPoint = "glFramebufferTextureLayer")]
        public static extern void glFramebufferTextureLayer(int target, int attachment, int texture, int level, int layer);

        [DllImport(LIB, EntryPoint = "glMapBufferRange")]
        public static extern void* glMapBufferRange(int target, IntPtr offset, IntPtr length, int access);

        [DllImport(LIB, EntryPoint = "glFlushMappedBufferRange")]
        public static extern void glFlushMappedBufferRange(int target, IntPtr offset, IntPtr length);

        [DllImport(LIB, EntryPoint = "glBindVertexArray")]
        public static extern void glBindVertexArray(int array);

        [DllImport(LIB, EntryPoint = "glDeleteVertexArrays")]
        public static extern void glDeleteVertexArrays(int n, int* arrays);

        [DllImport(LIB, EntryPoint = "glGenVertexArrays")]
        public static extern void glGenVertexArrays(int n, int* arrays);

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
        public static extern void glVertexAttribIPointer(int index, int size, int type, int stride, void* pointer);

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
        public static extern void glUniform1ui(int location, int v0);

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

        public const int GL_FRONT = 0x0404;
        public const int GL_BACK = 0x0405;
        public const int GL_FRONT_AND_BACK = 0x0408;

        public const int GL_INVALID_ENUM = 0x0500;
        public const int GL_INVALID_VALUE = 0x0501;
        public const int GL_INVALID_OPERATION = 0x0502;
        public const int GL_OUT_OF_MEMORY = 0x0505;

        public const int GL_CW = 0x0900;
        public const int GL_CCW = 0x0901;

        public const int GL_LINE_WIDTH = 0x0B21;
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
        public const int GL_SCISSOR_BOX = 0x0C10;
        public const int GL_SCISSOR_TEST = 0x0C11;
        public const int GL_COLOR_CLEAR_VALUE = 0x0C22;
        public const int GL_COLOR_WRITEMASK = 0x0C23;

        public const int GL_UNPACK_ALIGNMENT = 0x0CF5;
        public const int GL_PACK_ALIGNMENT = 0x0D05;

        public const int GL_RED_BITS = 0x0D52;
        public const int GL_GREEN_BITS = 0x0D53;
        public const int GL_BLUE_BITS = 0x0D54;
        public const int GL_ALPHA_BITS = 0x0D55;
        public const int GL_DEPTH_BITS = 0x0D56;
        public const int GL_STENCIL_BITS = 0x0D57;

        public const int GL_TEXTURE_2D = 0x0DE1;

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
        public const int GL_FIXED = 0x140C;

        public const int GL_DEPTH_COMPONENT = 0x1902;
        public const int GL_ALPHA = 0x1906;
        public const int GL_RGB = 0x1907;
        public const int GL_RGBA = 0x1908;
        public const int GL_LUMINANCE = 0x1909;
        public const int GL_LUMINANCE_ALPHA = 0x190A;

        public const int GL_UNSIGNED_SHORT_4_4_4_4 = 0x8033;
        public const int GL_UNSIGNED_SHORT_5_5_5_1 = 0x8034;
        public const int GL_UNSIGNED_SHORT_5_6_5 = 0x8363;

        public const int GL_FRAGMENT_SHADER = 0x8B30;
        public const int GL_VERTEX_SHADER = 0x8B31;
        public const int GL_MAX_VERTEX_ATTRIBS = 0x8869;
        public const int GL_MAX_VERTEX_UNIFORM_VECTORS = 0x8DFB;
        public const int GL_MAX_VARYING_VECTORS = 0x8DFC;
        public const int GL_MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D;
        public const int GL_MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C;
        public const int GL_MAX_TEXTURE_IMAGE_UNITS = 0x8872;
        public const int GL_MAX_FRAGMENT_UNIFORM_VECTORS = 0x8DFD;
        public const int GL_SHADER_TYPE = 0x8B4F;
        public const int GL_DELETE_STATUS = 0x8B80;
        public const int GL_LINK_STATUS = 0x8B82;
        public const int GL_VALIDATE_STATUS = 0x8B83;
        public const int GL_ATTACHED_SHADERS = 0x8B85;
        public const int GL_ACTIVE_UNIFORMS = 0x8B86;
        public const int GL_ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87;
        public const int GL_ACTIVE_ATTRIBUTES = 0x8B89;
        public const int GL_ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A;
        public const int GL_SHADING_LANGUAGE_VERSION = 0x8B8C;
        public const int GL_CURRENT_PROGRAM = 0x8B8D;

        public const int GL_NEVER_GLES = 0x0200;

        public const int GL_STENCIL_INDEX8 = 0x8D48;

        public const int GL_BUFFER_SIZE = 0x8764;
        public const int GL_BUFFER_USAGE = 0x8765;

        public const int GL_STENCIL_BACK_FUNC = 0x8800;
        public const int GL_STENCIL_BACK_FAIL = 0x8801;
        public const int GL_STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802;
        public const int GL_STENCIL_BACK_PASS_DEPTH_PASS = 0x8803;

        public const int GL_RED_BITS_GLES = 0x0D52;

        public const int GL_IMPLEMENTATION_COLOR_READ_TYPE = 0x8B9A;
        public const int GL_IMPLEMENTATION_COLOR_READ_FORMAT = 0x8B9B;

        public const int GL_TEXTURE_2D_ARRAY = 0x8C1A;
        public const int GL_TEXTURE_3D = 0x806F;

        public const int GL_TEXTURE_BINDING_2D = 0x8069;
        public const int GL_TEXTURE_BINDING_3D = 0x806A;
        public const int GL_TEXTURE_BINDING_2D_ARRAY = 0x8C1D;

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

        public const int GL_DEPTH_STENCIL = 0x84F9;
        public const int GL_UNSIGNED_INT_24_8 = 0x84FA;
        public const int GL_DEPTH24_STENCIL8 = 0x88F0;

        public const int GL_UNSIGNED_NORMALIZED = 0x8C17;
        public const int GL_DRAW_FRAMEBUFFER_BINDING = 0x8CA6;
        public const int GL_READ_FRAMEBUFFER = 0x8CA8;
        public const int GL_DRAW_FRAMEBUFFER = 0x8CA9;
        public const int GL_READ_FRAMEBUFFER_BINDING = 0x8CAA;

        public const int GL_RENDERBUFFER_SAMPLES = 0x8CAB;

        public const int GL_FRAMEBUFFER_INCOMPLETE_MULTISAMPLE = 0x8D56;

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

        public const int GL_FRAMEBUFFER_COMPLETE = 0x8CD5;
        public const int GL_FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 0x8CD6;
        public const int GL_FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7;
        public const int GL_FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER = 0x8CDB;
        public const int GL_FRAMEBUFFER_INCOMPLETE_READ_BUFFER = 0x8CDC;
        public const int GL_FRAMEBUFFER_UNSUPPORTED = 0x8CDD;

        public const int GL_MAX_RENDERBUFFER_SIZE = 0x84E8;

        public const int GL_DEPTH_COMPONENT16 = 0x81A5;
        public const int GL_DEPTH_COMPONENT24 = 0x81A6;
        public const int GL_DEPTH_COMPONENT32F = 0x8CAC;
        public const int GL_DEPTH32F_STENCIL8 = 0x8CAD;

        public const int GL_HALF_FLOAT = 0x140B;

        public const int GL_MAP_READ_BIT = 0x0001;
        public const int GL_MAP_WRITE_BIT = 0x0002;
        public const int GL_MAP_INVALIDATE_RANGE_BIT = 0x0004;
        public const int GL_MAP_INVALIDATE_BUFFER_BIT = 0x0008;
        public const int GL_MAP_FLUSH_EXPLICIT_BIT = 0x0010;
        public const int GL_MAP_UNSYNCHRONIZED_BIT = 0x0020;

        public const int GL_R8 = 0x8229;
        public const int GL_RG8 = 0x822B;
        public const int GL_RGB8 = 0x8051;
        public const int GL_RGBA8 = 0x8058;

        public const int GL_SRGB8_ALPHA8 = 0x8C43;

        public const int GL_RGB10_A2 = 0x8059;
        public const int GL_RGB10_A2UI = 0x906F;

        public const int GL_TEXTURE_2D_MULTISAMPLE = 0x9100;

        public const int GL_MAX_SAMPLES = 0x8D57;

        public const int GL_UNIFORM_BUFFER = 0x8A11;
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
        public const int GL_ACTIVE_UNIFORM_BLOCKS = 0x8A36;
        public const int GL_UNIFORM_TYPE = 0x8A37;
        public const int GL_UNIFORM_SIZE = 0x8A38;
        public const int GL_UNIFORM_NAME_LENGTH = 0x8A39;
        public const int GL_UNIFORM_BLOCK_INDEX = 0x8A3A;
        public const int GL_UNIFORM_BLOCK_BINDING = 0x8A3F;
        public const int GL_UNIFORM_BLOCK_DATA_SIZE = 0x8A40;
        public const int GL_UNIFORM_BLOCK_NAME_LENGTH = 0x8A41;
        public const int GL_UNIFORM_BLOCK_ACTIVE_UNIFORMS = 0x8A42;
        public const int GL_UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES = 0x8A43;

        public const int GL_TEXTURE_COMPARE_MODE = 0x884C;
        public const int GL_TEXTURE_COMPARE_FUNC = 0x884D;
        public const int GL_COMPARE_REF_TO_TEXTURE = 0x884E;

        public const int GL_VERTEX_ARRAY_BINDING = 0x85B5;

        public const int GL_MAX_VERTEX_OUTPUT_COMPONENTS = 0x9122;
        public const int GL_MAX_FRAGMENT_INPUT_COMPONENTS = 0x9125;

        public const int GL_TEXTURE_IMMUTABLE_FORMAT = 0x912F;

        public const int GL_ANY_SAMPLES_PASSED = 0x8C2F;
        public const int GL_ANY_SAMPLES_PASSED_CONSERVATIVE = 0x8D6A;

        public const int GL_SAMPLER_BINDING = 0x8919;

        public const int GL_RGB32F = 0x8815;
        public const int GL_RGBA32F = 0x8814;
        public const int GL_RGB16F = 0x881B;
        public const int GL_RGBA16F = 0x881A;

        public const int GL_R11F_G11F_B10F = 0x8C3A;

        public const int GL_UNSIGNED_INT_2_10_10_10_REV = 0x8368;

        public const int GL_MAX_VERTEX_UNIFORM_COMPONENTS = 0x8B4A;
        public const int GL_MAX_FRAGMENT_UNIFORM_COMPONENTS = 0x8B49;

        public const int GL_COLOR = 0x1800;
        public const int GL_DEPTH = 0x1801;
        public const int GL_STENCIL = 0x1802;

        public const int GL_RED = 0x1903;
        public const int GL_RG = 0x8227;

        public const int GL_TEXTURE_SWIZZLE_R = 0x8E42;
        public const int GL_TEXTURE_SWIZZLE_G = 0x8E43;
        public const int GL_TEXTURE_SWIZZLE_B = 0x8E44;
        public const int GL_TEXTURE_SWIZZLE_A = 0x8E45;

        public const int GL_COPY_READ_BUFFER = 0x8F36;
        public const int GL_COPY_WRITE_BUFFER = 0x8F37;

        public const int GL_TRANSFORM_FEEDBACK = 0x8E22;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER = 0x8C8E;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_BINDING = 0x8C8F;

        public const int GL_PRIMITIVES_GENERATED = 0x8C87;
        public const int GL_TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 0x8C88;

        public const int GL_RASTERIZER_DISCARD = 0x8C89;

        public const int GL_MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 0x8C8A;
        public const int GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 0x8C8B;
        public const int GL_MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 0x8C80;

        public const int GL_TRANSFORM_FEEDBACK_VARYINGS = 0x8C83;
        public const int GL_TRANSFORM_FEEDBACK_BUFFER_MODE = 0x8C7F;
        public const int GL_TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 0x8C76;

        public const int GL_INTERLEAVED_ATTRIBS = 0x8C8C;
        public const int GL_SEPARATE_ATTRIBS = 0x8C8D;

        public const int GL_MAJOR_VERSION = 0x821B;
        public const int GL_MINOR_VERSION = 0x821C;
        public const int GL_NUM_EXTENSIONS = 0x821D;

        public const int GL_PROGRAM_BINARY_RETRIEVABLE_HINT = 0x8257;
        public const int GL_PROGRAM_BINARY_LENGTH = 0x8741;

        public const int GL_NUM_PROGRAM_BINARY_FORMATS = 0x87FE;
        public const int GL_PROGRAM_BINARY_FORMATS = 0x87FF;

        public const int GL_VERTEX_SHADER_BIT = 0x00000001;
        public const int GL_FRAGMENT_SHADER_BIT = 0x00000002;

        public const uint GL_ALL_SHADER_BITS = 0xFFFFFFFF;

        public const int GL_PROGRAM_SEPARABLE = 0x8258;
        public const int GL_ACTIVE_PROGRAM = 0x8259;

        public const int GL_SAMPLER_2D = 0x8B5E;
        public const int GL_SAMPLER_3D = 0x8B5F;
        public const int GL_SAMPLER_CUBE = 0x8B60;
        public const int GL_SAMPLER_2D_SHADOW = 0x8B62;
        public const int GL_SAMPLER_2D_ARRAY = 0x8DC1;
        public const int GL_SAMPLER_2D_ARRAY_SHADOW = 0x8DC4;
        public const int GL_SAMPLER_CUBE_SHADOW = 0x8DC5;

        public const int GL_INT_SAMPLER_2D = 0x8DCA;
        public const int GL_INT_SAMPLER_3D = 0x8DCB;
        public const int GL_INT_SAMPLER_CUBE = 0x8DCC;
        public const int GL_INT_SAMPLER_2D_ARRAY = 0x8DCF;

        public const int GL_UNSIGNED_INT_SAMPLER_2D = 0x8DD2;
        public const int GL_UNSIGNED_INT_SAMPLER_3D = 0x8DD3;
        public const int GL_UNSIGNED_INT_SAMPLER_CUBE = 0x8DD4;
        public const int GL_UNSIGNED_INT_SAMPLER_2D_ARRAY = 0x8DD7;

        public const int GL_MAX_SAMPLES_GLES = 0x8D57;

        public const int GL_IMAGE_2D = 0x904D;
        public const int GL_IMAGE_3D = 0x904E;
        public const int GL_IMAGE_2D_ARRAY = 0x9053;
        public const int GL_INT_IMAGE_2D = 0x9058;
        public const int GL_INT_IMAGE_3D = 0x9059;
        public const int GL_INT_IMAGE_2D_ARRAY = 0x905E;
        public const int GL_UNSIGNED_INT_IMAGE_2D = 0x9063;
        public const int GL_UNSIGNED_INT_IMAGE_3D = 0x9064;
        public const int GL_UNSIGNED_INT_IMAGE_2D_ARRAY = 0x9069;

        public const int GL_MAX_IMAGE_UNITS = 0x8F38;
        public const int GL_MAX_VERTEX_IMAGE_UNIFORMS = 0x90CA;
        public const int GL_MAX_FRAGMENT_IMAGE_UNIFORMS = 0x90CE;
        public const int GL_MAX_COMBINED_IMAGE_UNIFORMS = 0x90CF;

        public const int GL_COMPRESSED_RGB8_ETC2 = 0x9274;
        public const int GL_COMPRESSED_SRGB8_ETC2 = 0x9275;
        public const int GL_COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9276;
        public const int GL_COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9277;
        public const int GL_COMPRESSED_RGBA8_ETC2_EAC = 0x9278;
        public const int GL_COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 0x9279;

        public const int GL_COMPRESSED_R11_EAC = 0x9270;
        public const int GL_COMPRESSED_SIGNED_R11_EAC = 0x9271;
        public const int GL_COMPRESSED_RG11_EAC = 0x9272;
        public const int GL_COMPRESSED_SIGNED_RG11_EAC = 0x9273;

        public const uint GL_COLOR_BUFFER_BIT = 0x00004000;
        public const uint GL_DEPTH_BUFFER_BIT = 0x00000100;
        public const uint GL_STENCIL_BUFFER_BIT = 0x00000400;

        public const int GL_SYNC_GPU_COMMANDS_COMPLETE = 0x9117;
        public const int GL_TIMEOUT_EXPIRED = 0x911B;
        public const int GL_CONDITION_SATISFIED = 0x911C;
        public const ulong GL_TIMEOUT_IGNORED = 0xFFFFFFFFFFFFFFFFUL;

        public const int GL_SYNC_STATUS = 0x9114;
        public const int GL_SIGNALED = 0x9119;
        public const int GL_UNSIGNALED = 0x9118;

        public const int GL_OBJECT_TYPE = 0x9112;

        public const int GL_BLEND = 0x0BE2;

        public const int GL_STATIC_DRAW = 0x88E4;
        public const int GL_DYNAMIC_DRAW = 0x88E8;
        public const int GL_STREAM_DRAW = 0x88E0;
    }
}
