using CoreAnimation;
using GLKit;
using OpenGLES;

namespace Engine.IOS
{
    public class GLViewController : UIViewController, IWindow  
    {
        private EAGLContext _context;
        private BinaryReader _reader;
        private LaminarEngine _engine;
        private GLKView _view;
        private CADisplayLink _displayLink;

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

        public void SetWindowSize(int width, int height) { }

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
                EAGLContext.SetCurrentContext(_context);
                _view.BindDrawable();

                Width = PhysicalWidth  = (int)_view.DrawableWidth;
                Height = PhysicalHeight = (int)_view.DrawableHeight;

                Debug.Log($"Size: {Width}x{Height}");

                if (_engine == null)
                    _engine = new LaminarEngine(this, ExecutableEntry.GetApplicationLayer(), _inputTest, _reader);

                StartDisplayLink();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            StopDisplayLink();
        }

        private void StartDisplayLink()
        {
            StopDisplayLink(); // avoid double-creating
            _displayLink = CADisplayLink.Create(OnFrame);
            _displayLink.PreferredFramesPerSecond = 60;
            _displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Default);
            Debug.Log("DisplayLink started");
        }

        private void StopDisplayLink()
        {
            _displayLink?.Invalidate();
            _displayLink = null;
        }

        private void OnFrame()
        {
            if (_engine == null || !_engine.IsInitialized)
                return;
            
            Debug.Log("OnFrame"); 
            EAGLContext.SetCurrentContext(_context);
            _view.BindDrawable();
            _engine?.Update();
            _view.Display();
        }

        private BinaryReader OpenBundleBinary(string relativePath)
        {
            var path = Path.Combine(NSBundle.MainBundle.ResourcePath, relativePath);
            return new BinaryReader(File.OpenRead(path));
        }

        public void SwapBuffers() { }
    }
}