using Metal;
using MetalKit;
using Foundation;
using UIKit;

namespace Engine.IOS
{
    public class MetalViewController : UIViewController, IMTKViewDelegate, IWindow
    {
            private BinaryReader _reader;
            private GFSEngine _engine;

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




        MTKView metalView;
        IMTLDevice device;
        IMTLCommandQueue commandQueue;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // _reader = OpenBundleBinary("Assets/GameData.gfs");

            // Create Metal device
            device = MTLDevice.SystemDefault;

            // Setup MTKView
            metalView = new MTKView(View.Bounds, device)
            {
                Delegate = this,
                EnableSetNeedsDisplay = true,
                PreferredFramesPerSecond = 60
            };
            View.AddSubview(metalView);

              //     Width = (int)view.DrawableWidth;
                //     Height = (int)view.DrawableHeight;

                //     PhysicalWidth  = (int)view.DrawableWidth;
                //     PhysicalHeight = (int)view.DrawableHeight;
                //     Debug.Log($"size ({Width}, {Height})");

                //     _engine = new GFSEngine(this, new GameApplication(), _inputTest, _reader);

            commandQueue = device.CreateCommandQueue();
        }

        private BinaryReader OpenBundleBinary(string relativePath)
        {
            var path = Path.Combine(NSBundle.MainBundle.ResourcePath, relativePath);
            return new BinaryReader(File.OpenRead(path));
        }

        // Called every frame
        public void Draw(MTKView view)
        {
            using var commandBuffer = commandQueue.CommandBuffer();
            using var renderPass = view.CurrentRenderPassDescriptor;
            
            if (renderPass != null)
            {
                using var encoder = commandBuffer.CreateRenderCommandEncoder(renderPass);
                encoder.EndEncoding();
                commandBuffer.PresentDrawable(view.CurrentDrawable);
            }

            commandBuffer.Commit();
        }

        public void DrawableSizeWillChange(MTKView view, CoreGraphics.CGSize size)
        {
            // Handle resize if needed
        }
        
        public void SetWindowSize(int width, int height)
        {
        }

        public void SwapBuffers()
        {
        }
    }

}