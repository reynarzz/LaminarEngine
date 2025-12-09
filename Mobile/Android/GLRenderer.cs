using Android.Opengl;
using Javax.Microedition.Khronos.Opengles;
using static OpenGL.ES.GLES30;

namespace Engine.Android
{
    public class GLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        private GFSEngine _engine;

        public void SetEngine(GFSEngine engine)
        {
            _engine = engine;
        }

        public void OnDrawFrame(IGL10? gl)
        {
            //if(_engine != null)
            //{
            //    _engine.Update();
            //}
        }

        public void OnSurfaceChanged(IGL10? gl, int width, int height)
        {
            
        }

        public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            // Here manage context loss
        }
    }
}
