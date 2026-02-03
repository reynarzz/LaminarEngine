#if DESKTOP
using static OpenGL.GL;
#else
using static OpenGL.ES.GLES30;
#endif
using System.Runtime.CompilerServices;
using Engine;

namespace OpenGL
{
    public static class GLUtils
    {
        internal static void PrintGLErrors(string context = "", [CallerMemberName] string caller = "", [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
#if DEBUG
            int err;
            while ((err = glGetError()) != GL_NO_ERROR)
            {
                string msg = err switch
                {
                    GL_INVALID_ENUM => "GL_INVALID_ENUM",
                    GL_INVALID_VALUE => "GL_INVALID_VALUE",
                    GL_INVALID_OPERATION => "GL_INVALID_OPERATION",
                    GL_INVALID_FRAMEBUFFER_OPERATION => "GL_INVALID_FRAMEBUFFER_OPERATION",
                    GL_OUT_OF_MEMORY => "GL_OUT_OF_MEMORY",
                    _ => $"Unknown error: 0x{err:X}"
                };

                Debug.Error($"OpenGL Error{(string.IsNullOrEmpty(context) ? "" : $" ({context})")}: {msg} " +
                            $"[Caller: {caller}, File: {System.IO.Path.GetFileName(file)}, Line: {line}]");
            }
#endif
        }
    }
}