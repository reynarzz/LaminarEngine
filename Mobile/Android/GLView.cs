using Android.Content;
using Android.Opengl;

namespace Engine.Android
{
    public class GLView : GLSurfaceView, IWindow
    {
        private readonly GLRenderer _renderer;

        public GLView(Context context, GLRenderer renderer) : base(context)
        {
            // Request an OpenGL ES 3.0 context
            SetEGLContextClientVersion(3);

            _renderer = renderer;
            SetRenderer(renderer);

            // Render continuously
            RenderMode = Rendermode.Continuously;
            
        }

        public string Name { get; set; }
        public bool IsFullScreen { get; set; }
        public bool CursorVisible { get; set; }

        public Color StartWindowColor { get; }

        public bool ShouldClose { get;  }

        public int MonitorCount { get; set; }

        public bool IsInitialized { get; set; } = true;

        public bool CanResize { get; set; }

        public event Action<int, int> OnWindowChanged;
        public event Action OnWindowClose;

        public void SetWindowSize(int width, int height)
        {
            OnWindowChanged?.Invoke(width, height);
        }

        public void SwapBuffers()
        {

        }
    }
}