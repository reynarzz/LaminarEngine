using Android.Content;
using Android.Opengl;
using Android.Runtime;
using Android.Views;

namespace Engine.Android
{
    public partial class GLView : GLSurfaceView, IWindow
    {

        public string Name { get; set; }
        public bool IsFullScreen { get; set; }
        public bool CursorVisible { get; set; }

        public Color StartWindowColor { get; }

        public bool ShouldClose { get; }

        public int MonitorCount { get; set; }

        public bool IsInitialized { get; set; } = true;

        public bool CanResize { get; set; }

        public event Action<int, int> OnWindowChanged;
        public event Action OnWindowClose;
        public int PhysicalWidth { get; set; }
        public int PhysicalHeight { get; set; }
        private readonly GLRenderer _renderer;
        private readonly AndroidSystem _system;

        public GLView(Context context) : base(context)
        {
            // Request an OpenGL ES 3.0 context
            SetEGLContextClientVersion(3);
            Focusable = true;
            FocusableInTouchMode = true;
            _system = new AndroidSystem(this);
            _renderer = new GLRenderer(this, _system);
            SetRenderer(_renderer);

            // Render continuously
            RenderMode = Rendermode.Continuously;
            PreserveEGLContextOnPause = true;

            PhysicalWidth = Width;
            PhysicalHeight = Height;
        }

        public void UpdateView(int width, int height)
        {
            PhysicalWidth = width;
            PhysicalHeight = height;
            OnWindowChanged?.Invoke(width, height);
        }
        public override void OnPause()
        {

            base.OnPause();
        }

        public override void OnResume()
        {
            base.OnResume();
        }

        public override bool OnTouchEvent(MotionEvent? e)
        {
            return _system.OnTouchEvent(e);
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            return _system.OnKeyDown(keyCode, e);
        }

        public override bool OnKeyUp([GeneratedEnum] Keycode keyCode, KeyEvent? e)
        {
            return _system.OnKeyUp(keyCode, e);
        }

        public override bool OnGenericMotionEvent(MotionEvent? e)
        {
            return _system.OnGenericMotionEvent(e);
        }

        public void SetWindowSize(int width, int height)
        {
            // Android doesn't implement this.
        }

        public void SwapBuffers() { }
    }
}