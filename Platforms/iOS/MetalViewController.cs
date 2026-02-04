using CoreAnimation;
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


        private MTKView _metalView;
        private CAMetalLayer _metalLayer;
        private IMTLDevice device;
        private IMTLCommandQueue commandQueue;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // _reader = OpenBundleBinary("Assets/GameData.gfs");
            Console.WriteLine("View loaded");
            // Create Metal device
            device = MTLDevice.SystemDefault;

            // Setup MTKView
            _metalView = new MTKView(View.Bounds, device)
            {
                Delegate = this,
                EnableSetNeedsDisplay = true,
                PreferredFramesPerSecond = 60
            };

            _metalLayer = new CAMetalLayer
            {
                Device = device,
                PixelFormat = MTLPixelFormat.BGRA8Unorm,
                FramebufferOnly = true,
                Frame = View.Layer.Bounds,
                ContentsScale = UIScreen.MainScreen.Scale
            };

            View.AddSubview(_metalView);

            Width = (int)View.Bounds.Width;
            Height = (int)View.Bounds.Height;

            PhysicalWidth = Width;
            PhysicalHeight = Height;
            Debug.Log($"size ({Width}, {Height})");

            //     _engine = new GFSEngine(this, new GameApplication(), _inputTest, _reader);

            commandQueue = device.CreateCommandQueue();
        }

        private BinaryReader OpenBundleBinary(string relativePath)
        {
            var path = Path.Combine(NSBundle.MainBundle.ResourcePath, relativePath);
            return new BinaryReader(File.OpenRead(path));
        }

        public void Draw(MTKView view)
        {
            using var commandBuffer = commandQueue.CommandBuffer();
            using var renderPass = view.CurrentRenderPassDescriptor;

            if (renderPass != null)
            {
                var drawable = _metalLayer.NextDrawable();

                renderPass.ColorAttachments[0].Texture = drawable.Texture;
                renderPass.ColorAttachments[0].LoadAction = MTLLoadAction.Clear;
                renderPass.ColorAttachments[0].StoreAction = MTLStoreAction.Store;
                renderPass.ColorAttachments[0].ClearColor = new MTLClearColor(1, 0, 0, 1); // blue
                Debug.Log("Drawing");
                view.ClearColor = new MTLClearColor(1, 0, 0, 1);
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