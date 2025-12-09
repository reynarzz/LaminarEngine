using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using static OpenGL.ES.GLES30;

namespace Engine.Android
{
    public class GLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        public void OnDrawFrame(IGL10? gl)
        {
            glClearColor(0, 1, 0, 1);
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
        }

        public void OnSurfaceChanged(IGL10? gl, int width, int height)
        {
            unsafe
            {
                int a = 0;
                glGenVertexArrays(1, &a);
                glBindVertexArray(a);
                glCreateProgram();
            }
        }

        public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            // Here manage context loss
        }
    }
}
