using Editor.Utils;
using Engine;
using Engine.Graphics;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using System.Numerics;

namespace Editor
{
    public enum GameViewResolution
    {
        FreeAspect,
        Resolution
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

        public nint NativeWindow => 0;

        private vec2 _targetResolution = new vec2(1024, 576);
        private float _targetResScale = 1.0f;
        private GameViewResolution _resolutionType = GameViewResolution.Resolution;
        private const float TOOLBAR_HEIGHT = 28;
        private bool _autoFit = true;
        private const float MAX_VIEW_SCALE = 7.0f;

        private readonly string[] _resolutionTypesNames;
        public EditorGameView(IWindow window, RenderingSurface surface, InputLayerBase inputLayer) : base("Game", surface)
        {
            _window = window;
            _width = _window.Width;
            _height = _window.Height;
            _inputLayer = inputLayer;

            _resolutionTypesNames = Enum.GetNames(typeof(GameViewResolution));
        }

        public void SetWindowSize(int width, int height)
        {

        }

        public void SwapBuffers()
        {
        }

        protected override vec2 GetViewSize()
        {
            if (_resolutionType == GameViewResolution.FreeAspect)
            {
                return new vec2(_width, _height) * _targetResScale;
            }

            return _targetResolution * _targetResScale;
        }

        protected override vec2 GetViewPosition()
        {
            var avail = ImGui.GetContentRegionAvail().ToVec2();
            var cursor = ImGui.GetCursorPos().ToVec2();

            var size = new vec2(_width, _height) * _targetResScale;
            var pos = cursor + (avail - size) * 0.5f;

            if (_resolutionType == GameViewResolution.FreeAspect)
            {
                pos.y += (int)ImGui.GetFrameHeight();
            }
            return pos;
        }

        protected override void OnWindowRender()
        {
            _inputLayer.IsEnabled = ImGui.IsWindowFocused();
        }

        protected override void OnRenderChildWindows()
        {
            Toolbar();
        }

        private void Toolbar()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 2));
            ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, 0);

            ImGui.BeginChild("##GameViewChild", new Vector2(ImGui.GetContentRegionAvail().X, TOOLBAR_HEIGHT), ImGuiChildFlags.AlwaysUseWindowPadding,
                             ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoDecoration);
            var targetRes = Mathf.RoundToInt(_targetResolution);
            var currentResIndex = (int)_resolutionType;
            ImGui.SameLine();
            ImGui.SetNextItemWidth(135);
            if (ImGui.Combo("##GameViewRes", ref currentResIndex, _resolutionTypesNames, _resolutionTypesNames.Length))
            {
                _resolutionType = (GameViewResolution)currentResIndex;

                if(_resolutionType == GameViewResolution.FreeAspect && _targetResScale < 1.0f)
                {
                    _targetResScale = 1.0f;
                }
                OnImguiWindowSizeChanged();
            }
            ImGui.BeginDisabled(_resolutionType != GameViewResolution.Resolution);
            ImGui.SameLine();
            if (_resolutionType == GameViewResolution.FreeAspect)
            {
                targetRes = new ivec2(_width, _height);
            }
            if (EditorGuiFieldsResolver.DrawIVec2FieldTrueWidth("Resolution", ref targetRes, 100))
            {
                _targetResolution = new vec2(targetRes.x, targetRes.y);
                OnImguiWindowSizeChanged();
            }
            ImGui.EndDisabled();

            ImGui.SameLine();
            var scale = _targetResScale;
            var min = _resolutionType == GameViewResolution.Resolution? CalculateAutoFitScale(_targetResolution):1;
            ImGui.Text("Scale");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(170);

            void RecalcScale(float scale)
            {
                _targetResScale = Mathf.Clamp(scale, min, MAX_VIEW_SCALE);
                _autoFit = Mathf.CompareFloats(_targetResScale, min, 0.05f);

            }
            if (ImGui.SliderFloat("##Scale", ref scale, min, MAX_VIEW_SCALE, "%.2f"))
            {
                //if(_targetResScale > min)
                //{
                //    WindowFlags &= ~ImGuiWindowFlags.NoScrollbar;
                //}
                //else
                //{
                //    WindowFlags |= ImGuiWindowFlags.NoScrollbar;
                //}

                RecalcScale(scale);
            }
            ImGui.SameLine();

            if(EditorGuiFieldsResolver.DrawFloatFieldRealWidth("##Scale_Value", ref scale, 40, min, MAX_VIEW_SCALE))
            {
                RecalcScale(scale);
            }
            ImGui.SameLine();
            if (ImGui.Button("Stats"))
            {

            }
            ImGui.EndChild();
            ImGui.PopStyleVar(3);
        }
        public override void OnDraw()
        {
            // Draw the window
            base.OnDraw();

            _offsetX = Mathf.RoundToInt(WindowPositionRender.X);
            var frameOffset = 0;

            if (_resolutionType == GameViewResolution.FreeAspect)
            {
                frameOffset = (int)ImGui.GetFrameHeight() / 2;
            }
            _offsetY = Mathf.RoundToInt(WindowPositionRender.Y - frameOffset);

        }

        protected override void OnImguiWindowSizeChanged()
        {
            if (_resolutionType == GameViewResolution.FreeAspect)
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

            if ((avail.x >= targetResolution.x && avail.y >= targetResolution.y) ||
                targetResolution.x <= 0 || targetResolution.y <= 0)
            {
                return 1;
            }

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
