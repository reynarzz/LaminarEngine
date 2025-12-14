using Game;
using GLKit;
using OpenGL;
using OpenGLES;
using static OpenGL.ES.GLES30;

namespace Engine.IOS
{

    public class GLViewController : GLKViewController, IWindow
    {
        private EAGLContext _context;
        private BinaryReader _reader;
        private GFSEngine _engine;

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

        public int Width { get; set; }

        public int Height { get; set; }

        public void SetWindowSize(int width, int height)
        {
        }
        public void UpdateView(int width, int height)
        {
            PhysicalWidth = width;
            PhysicalHeight = height;

            OnWindowChanged?.Invoke(width, height);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create an OpenGL ES 3.0 context
            _context = new EAGLContext(EAGLRenderingAPI.OpenGLES3);

            var glkView = new GLKView(View.Frame, _context)
            {
                DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24,

            };

            View = glkView;

            EAGLContext.SetCurrentContext(_context);

            _reader = OpenBundleBinary("Assets/GameData.gfs");

            var view = (GLKView)View;

            nfloat widthPoints = view.Bounds.Width;
            nfloat heightPoints = view.Bounds.Height;

            nfloat scale = UIScreen.MainScreen.Scale;

            Width = (int)(widthPoints * scale);
            Height = (int)(heightPoints * scale);

            PhysicalWidth = Width;
            PhysicalHeight = Height;
        }

        private BinaryReader OpenBundleBinary(string relativePath)
        {
            var path = Path.Combine(NSBundle.MainBundle.BundlePath, relativePath);
            return new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read));
        }
        
        public override void Update()
        {
            // Game update logic here
        }

        public override void DrawInRect(GLKView view, CGRect rect)
        {
            if (_engine == null)
            {
                _engine = new GFSEngine(this, new GameApplication(), _reader);
            }
            else
            {
                _engine.Update();
            }
        }

        public void SwapBuffers()
        {
        }
    }
}