using Engine;
using Engine.Graphics;
using Engine.Layers;
using ImGuiNET;
using System.Numerics;

namespace Editor
{
    internal class EditorGameView : EditorRenderSurfaceView, IWindow
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
        public EditorGameView(WindowStandalone window, RenderingLayer.RenderingSurface surface) : base("Game", surface)
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

        public override void OnRender()
        {
            base.OnRender();

            _offsetX = (int)WindowPosition.X;
            _offsetY = (int)WindowPosition.Y;

            if (_width != (int)WindowSize.X || _height != (int)WindowSize.Y)
            {
                _width = (int)WindowSize.X;
                _height = (int)WindowSize.Y;

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
