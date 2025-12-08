using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;

public class GLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
{
    public void OnDrawFrame(IGL10? gl)
    {
        GLES30.GlClearColor(1, 0, 0, 1);
        GLES30.GlClear(GLES30.GlColorBufferBit |  GLES30.GlDepthBufferBit);
    }

    public void OnSurfaceChanged(IGL10? gl, int width, int height)
    {
    }

    public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
    {
        // Here manage context loss
    }
}
