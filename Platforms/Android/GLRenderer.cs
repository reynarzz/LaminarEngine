using Android.Content.Res;
using Android.Opengl;
using Game;
using Javax.Microedition.Khronos.Opengles;
using Engine;
using System.Text;

namespace Engine.Android
{
    public class GLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
    {
        private readonly GLView _glView;
        private readonly AndroidSystem _system;
        public GLRenderer(GLView view, AndroidSystem system)
        {
            _glView = view;
            _system = system;
        }

        public void OnSurfaceChanged(IGL10? gl, int width, int height)
        {
            _glView.UpdateView(width, height);
            _system.OnSurfaceChanged(width, height);
        }

        public void OnSurfaceCreated(IGL10? gl, Javax.Microedition.Khronos.Egl.EGLConfig? config)
        {
            _system.OnSurfaceCreated(config);
        }

        public void OnDrawFrame(IGL10? gl)
        {
            _system.OnDrawFrame();
        }
    }
}
