using Engine;
using Engine.Graphics;
using Engine.Utils;
using GlmNet;
using ImGuiNET;

namespace Editor
{
    public enum GameViewResolution
    {
        Free,
        Custom
    }

    internal class EditorGameView : EditorRenderSurfaceView, IWindow
    {
        private readonly InputLayerBase _inputLayer;
        public string Name { get; set; } = "Game View";
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
        private readonly IWindow _window;
        private bool _updateWindowSize;
        private int _offsetX = 0;
        private int _offsetY = 0;
        public int OffsetX => _offsetX;
        public int OffsetY => _offsetY;
        private vec2 _targetResolution = new vec2(512 * 2, 258 * 2);
        private float _targetResScale = 1.0f;
        private GameViewResolution _resolutionType = GameViewResolution.Custom;
        private bool _autoFit = true;
        public EditorGameView(IWindow window, RenderingSurface surface, InputLayerBase inputLayer) : base("Game", surface)
        {
            _window = window;
            _width = _window.Width;
            _height = _window.Height;
            _inputLayer = inputLayer;
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

        protected override vec2 GetViewSize()
        {
            if (_resolutionType == GameViewResolution.Free)
            {
                return base.GetViewSize();
            }

            return _targetResolution * _targetResScale;
        }

        protected override vec2 GetViewPosition()
        {
            var avail = ImGui.GetContentRegionAvail().ToVec2();
            var cursor = ImGui.GetCursorPos().ToVec2();

            var size = new vec2(_width, _height) * _targetResScale;
            var pos = cursor + (avail - size) * 0.5f;

            if (_resolutionType == GameViewResolution.Free)
            {
                pos.y += (int)ImGui.GetFrameHeight() / 2;
            }
            return pos;
        }

        protected override void OnWindowRender()
        {
            _inputLayer.IsEnabled = ImGui.IsWindowFocused();
        }

        public override void OnDraw()
        {
            base.OnDraw();

            _offsetX = Mathf.RoundToInt(WindowPositionRender.X);
            var frameOffset = 0;

            if (_resolutionType == GameViewResolution.Free)
            {
                frameOffset = (int)ImGui.GetFrameHeight() / 2;
            }
            _offsetY = Mathf.RoundToInt(WindowPositionRender.Y - frameOffset);

        }

        protected override void OnImguiWindowSizeChanged()
        {
            if (_resolutionType == GameViewResolution.Free)
            {
                TryNotifyUpdateResolution(WindowSize.ToVec2());
            }
            else
            {
                if (_autoFit)
                {
                    // TODO: only call this when window size changes.
                    _targetResScale = CalculateAutoFitScale(_targetResolution);
                }

                TryNotifyUpdateResolution(_targetResolution);
            }
        }

        private float CalculateAutoFitScale(vec2 targetResolution)
        {
            var avail = GetPrevContentRegionAvail();

            if (avail.x >= targetResolution.x && avail.y >= targetResolution.y)
                return 1;

            if (targetResolution.x <= 0 || targetResolution.y <= 0)
                return 1.0f;

            var scaleX = avail.x / targetResolution.x;
            var scaleY = avail.y / targetResolution.y;

            return MathF.Min(scaleX, scaleY);
        }

        private void TryNotifyUpdateResolution(vec2 newResolution)
        {
            var newX = Mathf.RoundToInt(newResolution.x);
            var newY = Mathf.RoundToInt(newResolution.y);

            if (_width != newX || _height != newY)
            {
                _width = newX;
                _height = newY;

                _updateWindowSize = true;
            }
        }

        public override void OnUpdate()
        {
            if (_updateWindowSize)
            {
                _updateWindowSize = false;
                OnWindowChanged?.Invoke(_width, _height);
            }
        }
    }
}
