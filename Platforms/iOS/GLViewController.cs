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
                DrawableColorFormat = GLKViewDrawableColorFormat.RGBA8888,
                DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24,
                DrawableStencilFormat = GLKViewDrawableStencilFormat.Format8
            };

            if (!EAGLContext.SetCurrentContext(_context))
            {
                Debug.Error("Cannot create opengl context");
            }

            // Force drawable creation
            glkView.BindDrawable();

            View = glkView;
             Debug.Prefix = "com.reynarzz.gfs:CONSOLE ";
            _reader = OpenBundleBinary("Assets/GameData.gfs");

            nfloat widthPoints = glkView.Bounds.Width;
            nfloat heightPoints = glkView.Bounds.Height;

            nfloat scale = UIScreen.MainScreen.Scale;

            Width = (int)(widthPoints * scale);
            Height = (int)(heightPoints * scale);

            PhysicalWidth = Width;
            PhysicalHeight = Height;
           
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            var glkView = (GLKView)View;

            EAGLContext.SetCurrentContext(_context);

            // Force drawable creation
            glkView.BindDrawable();
           
        }

        private BinaryReader OpenBundleBinary(string relativePath)
        {
            var path = Path.Combine(NSBundle.MainBundle.ResourcePath, relativePath);
            return new BinaryReader(File.OpenRead(path));
        }

        int ReadInt32BE(BinaryReader r)
        {
            var b = r.ReadBytes(4);
            Array.Reverse(b);
            return BitConverter.ToInt32(b, 0);
        }
        
        public override void Update()
        {
            // Game update logic here
        }

        public override void DrawInRect(GLKView view, CGRect rect)
        {
            if (_engine == null)
            {
                unsafe
                {
                     int fbo = 0;
                glGetIntegerv(GL_FRAMEBUFFER_BINDING, &fbo);
                glBindFramebuffer(GL_FRAMEBUFFER, (uint)fbo);
                 glBindFramebuffer(GL_DRAW_FRAMEBUFFER, (uint)fbo);
               
                }
               
              
                try
                {
                    _engine = new GFSEngine(this, new GameApplication(), _reader);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
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