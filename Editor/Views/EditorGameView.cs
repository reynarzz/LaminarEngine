using Engine;
using Engine.Graphics;
using Engine.Layers;
using ImGuiNET;
using System.Numerics;

namespace Editor
{
    internal class EditorGameView : IWindow
    {
        public string Name { get; set; } = "Editor";
        public bool IsFullScreen { get; set; }

        private int _width = 100;
        private int _height = 100;
        public int Width => _width;
        public int Height => _height;

        public bool CursorVisible { get; set; }

        private Color _startWindowColor = Color.Black;
        public Color StartWindowColor => _startWindowColor;

        private bool _shouldClose;
        public bool ShouldClose => _shouldClose;
        public int MonitorCount => 1;
        public bool IsInitialized => true;
        public bool CanResize { get; set; }
        public int PhysicalWidth => _width;
        public int PhysicalHeight => _height;

        public event Action<int, int> OnWindowChanged;
        public event Action OnWindowClose;
        private readonly WindowStandalone _window;
        private bool _updateWindowSize;
        private int _offsetX = 0;
        private int _offsetY = 0;
        public int OffsetX => _offsetX;
        public int OffsetY => _offsetY;
        private readonly RenderingLayer.RenderingSurface _surface;
        public EditorGameView(WindowStandalone window, RenderingLayer.RenderingSurface surface)
        {
            _window = window;
            _width = _window.Width;
            _height = _window.Height;
            _surface = surface;
            //_window.OnWindowChanged += OnWindowWasChanged;
        }

        private void OnWindowWasChanged(int w, int h)
        {
            _updateWindowSize = true;
        }

        public void SetWindowSize(int width, int height)
        {

        }

        public void SwapBuffers()
        {
        }

        public void Render()
        {
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoCollapse;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 1));
            ImGui.PushID("GameVIew");
            ImGui.Begin("Game", flags);
            var size = ImGui.GetWindowSize();
            var pos = ImGui.GetWindowPos();
            _offsetX = (int)pos.X;
            _offsetY = (int)pos.Y + (int)ImGui.GetFrameHeight() / 2;

            if (_surface.Cameras != null && _surface.Cameras.Length > 0)
            {
                var cameraRenderTarget = _surface.Cameras[0].OutRenderTexture?.NativeResource;

                if(cameraRenderTarget == null)
                {
                    cameraRenderTarget = _surface.RenderTexture.NativeResource;
                }

                var frameBuffer = cameraRenderTarget as GLFrameBuffer;
                ImGui.Image((nint)frameBuffer.ColorTexture.Handle, ImGui.GetContentRegionAvail(), new Vector2(0, 1), new Vector2(1, 0));
            }
            else
            {
                // TODO: move this to be calculated just once.
                string text = "No valid cameras were found/enabled.";
                Vector2 textSize = ImGui.CalcTextSize(text);
                Vector2 windowSize = ImGui.GetContentRegionAvail();

                ImGui.SetCursorPos(new Vector2((windowSize.X - textSize.X) * 0.5f, (windowSize.Y - textSize.Y) * 0.5f));

                ImGui.Text(text);
            }

            ImGui.End();
            ImGui.PopID();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();

            if (_width != (int)size.X || _height != (int)size.Y)
            {
                _width = (int)size.X;
                _height = (int)size.Y;

                _updateWindowSize = true;
            }
        }

        internal void Update()
        {
            if (_updateWindowSize)
            {
                _updateWindowSize = false;
                OnWindowChanged?.Invoke(_width, _height);
            }
        }
    }
}
