using System;
using CoreAnimation;
using Engine.Graphics;
using Foundation;
using GLKit;
using OpenGLES;
using UIKit;

namespace Engine.IOS
{
    public class GLViewController : GLKViewController, IWindow
    {
        private EAGLContext _context;
        private BinaryReader _reader;
        private LaminarEngine _engine;
        private GLKView _view;

        public string Name { get; set; } = "Iphone Window";
        public bool IsFullScreen { get; set; } = true;
        public bool CursorVisible { get; set; }
        public Color StartWindowColor { get; }
        public bool ShouldClose { get; }
        public int MonitorCount { get; set; } = 1;
        public bool IsInitialized { get; set; } = true;
        public bool CanResize { get; set; } = false;
        public event Action<int, int> OnWindowChanged;
        public event Action OnWindowClose;
        public int PhysicalWidth { get; set; }
        public int PhysicalHeight { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int OffsetX => 0;
        public int OffsetY => 0;
        public nint NativeWindow => 0;
        public override bool PrefersHomeIndicatorAutoHidden => true;
        
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
            _view.BackgroundColor = UIColor.Black;

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
            UIApplication.SharedApplication.IdleTimerDisabled = true;
            
            try
            {
                if (_engine != null)
                    return;
                EAGLContext.SetCurrentContext(_context);
                _view.BindDrawable();

                Width = PhysicalWidth = (int)_view.DrawableWidth;
                Height = PhysicalHeight = (int)_view.DrawableHeight;

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

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (_view == null)
                return;
        }

        public override void DrawInRect(GLKView view, CGRect rect)
        {
            EAGLContext.SetCurrentContext(_context);
            _view.BindDrawable();
            
            if(_engine == null)
                return;
           
            GLFrameBuffer.SyncDefaultFrameBuffer();

            try
            {
                _engine.Update();

                if (_view.DrawableWidth != Width || _view.DrawableHeight != Height)
                {
                    Width = PhysicalWidth = (int)_view.DrawableWidth;
                    Height = PhysicalHeight = (int)_view.DrawableHeight;
                    OnWindowChanged?.Invoke(Width, Height);
                }
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


        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            foreach (UITouch touch in touches)
            {
                var location = touch.LocationInView(_view);
                Debug.Log($"Touch began at {location.X}, {location.Y}");
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            foreach (UITouch touch in touches)
            {
                var location = touch.LocationInView(_view);
                Debug.Log($"Touch moved to {location.X}, {location.Y}");
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            Debug.Log("Touch ended");
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);


            // Hide the navigation bar
            NavigationController?.SetNeedsUpdateOfHomeIndicatorAutoHidden();
        }

        public void SwapBuffers()
        {
        }
    }
}
