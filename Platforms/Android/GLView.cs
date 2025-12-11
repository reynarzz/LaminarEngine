using Android.Content;
using Android.Opengl;
using Android.Views;

namespace Engine.Android
{
    public partial class GLView : GLSurfaceView, IWindow
    {
        private readonly GLRenderer _renderer;

        public GLView(Context context) : base(context)
        {
            // Request an OpenGL ES 3.0 context
            SetEGLContextClientVersion(3);
            Focusable = true;
            FocusableInTouchMode = true;

            _renderer = new GLRenderer(this);
            SetRenderer(_renderer);

            // Render continuously
            RenderMode = Rendermode.Continuously;

            PhysicalWidth = Width;
            PhysicalHeight = Height;
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
        public int PhysicalWidth { get; set; }
        public int PhysicalHeight { get; set; }

        public void UpdateView(int width, int height)
        {
            PhysicalWidth = width;
            PhysicalHeight = height;

            OnWindowChanged?.Invoke(width, height);
        }
        public void SetWindowSize(int width, int height)
        {
            // Android doesn't implement this.
        }

        public void SwapBuffers()
        {

        }
    }
}