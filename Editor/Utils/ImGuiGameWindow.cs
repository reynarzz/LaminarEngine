using Engine;
using Engine.Graphics;
using Engine.Layers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class ImGuiGameWindow : IWindow
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

        public ImGuiGameWindow(WindowStandalone window)
        {
            _window = window;
            _width = _window.Width;
            _height = _window.Height;
        }

        private void OnWindowWasChanged(int w, int h)
        {
            OnWindowChanged?.Invoke(_width, _height);
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
            ImGui.Begin("GameView", flags);
            var size = ImGui.GetWindowSize();

            RenderingLayer.RenderToScreen = false;
            var frameBuffer = (RenderingLayer.ScreenRenderTexture.NativeResource as GLFrameBuffer);

            ImGui.Image((nint)frameBuffer.ColorTexture.Handle, ImGui.GetContentRegionAvail(), new Vector2(0, 1), new Vector2(1, 0));
            ImGui.End();
            ImGui.PopStyleVar();


            if (_width != size.X || _height != size.Y)
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
