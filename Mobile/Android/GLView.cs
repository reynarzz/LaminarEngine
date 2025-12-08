using Android.Content;
using Android.Opengl;

namespace Engine.Android
{
    public class GLView : GLSurfaceView
    {
        private readonly GLRenderer _renderer;

        public GLView(Context context) : base(context)
        {
            // Request an OpenGL ES 3.0 context
            SetEGLContextClientVersion(3);

            _renderer = new GLRenderer();
            SetRenderer(_renderer);

            // Render continuously
            RenderMode = Rendermode.Continuously;
        }
    }
}