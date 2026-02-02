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
        private InputLayerIOS _inputTest = new();
        public nint NativeWindow => 0;
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


            try
            {
                // Create an OpenGL ES 3.0 context
                _context = new EAGLContext(EAGLRenderingAPI.OpenGLES3);

                // _view = new GLKView(View.Frame, _context)
                // {
                //     DrawableColorFormat = GLKViewDrawableColorFormat.RGBA8888,
                //     DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24,
                //     DrawableStencilFormat = GLKViewDrawableStencilFormat.Format8,
                //     MultipleTouchEnabled = true
                // };

                _view = (GLKView)View;
                _view.Context = _context;
                _view.DrawableColorFormat = GLKViewDrawableColorFormat.RGBA8888;
                _view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;
                _view.DrawableStencilFormat = GLKViewDrawableStencilFormat.Format8;
                _view.Alpha = 1;
                _view.MultipleTouchEnabled = true;
                // Hook up the rendering method.
                //

                // Configure the GLKViewController properties
                PreferredFramesPerSecond = 60;

                var layer = (CoreAnimation.CAEAGLLayer)_view.Layer;
                layer.Opaque = true;
                layer.Opacity = 1;
                layer.DrawableProperties = new NSDictionary(EAGLDrawableProperty.RetainedBacking, false, EAGLDrawableProperty.ColorFormat, EAGLColorFormat.RGBA8);


                Debug.Prefix = "com.reynarzz.gfs:CONSOLE ";
                _reader = OpenBundleBinary("Assets/GameData.gfs");

                nfloat widthPoints = _view.Bounds.Width;
                nfloat heightPoints = _view.Bounds.Height;

                nfloat scale = UIScreen.MainScreen.Scale;

                Width = (int)(widthPoints * scale);
                Height = (int)(heightPoints * scale);

                PhysicalWidth = Width;
                PhysicalHeight = Height;

                Debug.Log($"width: {Width}, Height: {Height}, Pwidth: {PhysicalWidth}, PHeight: {PhysicalHeight}, ----asdasd");

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

        }

        private BinaryReader OpenBundleBinary(string relativePath)
        {
            var path = Path.Combine(NSBundle.MainBundle.ResourcePath, relativePath);
            return new BinaryReader(File.OpenRead(path));
        }

        public override void Update()
        {
            // Game update logic here
        }
        // void Draw(object sender, GLKViewDrawEventArgs args)
        // {

        // }

        public override void DrawInRect(GLKView view, CGRect rect)
        {
            try
            {
                EAGLContext.SetCurrentContext(_context);
                view.BindDrawable();
                
                if (_engine == null)
                {
                    var status = glCheckFramebufferStatus(GL_FRAMEBUFFER);
                        Debug.Log("Is frame buffer ok?: " + (status == GL_FRAMEBUFFER_COMPLETE));

                    //if (status == GL_FRAMEBUFFER_COMPLETE)
                    {

                        _engine = new GFSEngine(this, new GameApplication(), _inputTest, _reader);

                    }
                }
                else
                {
                    _engine.Update();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void SwapBuffers()
        {
        }
    }
}