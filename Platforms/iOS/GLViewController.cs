using CoreAnimation;
using Engine.Graphics;
using GLKit;
using OpenGLES;
using static OpenGL.ES.GLES30;

namespace Engine.IOS
{
    public class GLViewController : GLKViewController, IWindow
    {
        private EAGLContext _context;
        private BinaryReader _reader;
        private LaminarEngine _engine;
        private GLKView _view;

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
        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX => 0;
        public int OffsetY => 0;
        public nint NativeWindow => 0;

        private InputLayerIOS _inputTest = new();

        public void SetWindowSize(int width, int height)
        {
        }

        public void UpdateView(int width, int height)
        {
            PhysicalWidth = width;
            PhysicalHeight = height;
            OnWindowChanged?.Invoke(width, height);
        }

        public override void LoadView()
        {
            _context = new EAGLContext(EAGLRenderingAPI.OpenGLES3);
            EAGLContext.SetCurrentContext(_context);

            _view = new GLKView(UIScreen.MainScreen.Bounds, _context)
            {
                DrawableColorFormat = GLKViewDrawableColorFormat.RGBA8888,
                DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24,
                DrawableStencilFormat = GLKViewDrawableStencilFormat.Format8,
                MultipleTouchEnabled = true,
                // Delegate = this, // required for DrawInRect to fire
            };

            var layer = (CAEAGLLayer)_view.Layer;
            layer.Opaque = true;

            View = _view;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            try
            {
                PreferredFramesPerSecond = 60;
                Paused = false;
                ResumeOnDidBecomeActive = true;

                _reader = OpenBundleBinary($"Assets/{Paths.ASSET_BUILD_DATA_FULL_FILE_NAME}");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            try
            {
                if (_engine != null)
                    return;

                EAGLContext.SetCurrentContext(_context);
                _view.BindDrawable();

                _newWidth = Width = PhysicalWidth = (int)_view.DrawableWidth;
               _newHeight = Height = PhysicalHeight = (int)_view.DrawableHeight;

                Debug.Log($"Size: {Width}x{Height}");

                _engine = new LaminarEngine(this, ExecutableEntry.GetApplicationLayer(), _inputTest, _reader);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }

            Paused = false;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            Paused = true; //pauses GLKViewController's internal display link
        }

        public override void Update()
        {
        }

        private int _newWidth;
        private int _newHeight;

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (_view == null)
                return;

            var newWidth = (int)_view.DrawableWidth;
            var newHeight = (int)_view.DrawableHeight;

            if (newWidth == 0 || newHeight == 0)
                return;
            if (newWidth == _newWidth && newHeight == _newHeight)
                return;

            _newWidth = newWidth;
            _newHeight = newHeight;
        }

        public override void DrawInRect(GLKView view, CGRect rect)
        {
            if (_engine == null)
                return;

            EAGLContext.SetCurrentContext(_context);
            _view.BindDrawable();
            GLFrameBuffer.SyncDefaultFrameBuffer();

            try
            {
                if (_newWidth != Width || _newHeight != Height)
                {
                    Width = PhysicalWidth = _newWidth;
                    Height = PhysicalHeight = _newHeight;
                    OnWindowChanged?.Invoke(Width, Height);
                }

                _engine.Update();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private BinaryReader OpenBundleBinary(string relativePath)
        {
            var path = Path.Combine(NSBundle.MainBundle.ResourcePath, relativePath);
            return new BinaryReader(File.OpenRead(path));
        }

        public void SwapBuffers()
        {
        }
    }
}