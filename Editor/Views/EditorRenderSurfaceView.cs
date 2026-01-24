using Editor.Views;
using Engine;
using Engine.Graphics;
using Engine.Layers;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal abstract class EditorRenderSurfaceView : IEditorWindow
    {
        private readonly RenderingSurface _surface;
        protected RenderingSurface Surface { get; }
        public Vector2 WindowPositionRender { get; private set; }
        public Vector2 WindowPosition { get; private set; }
        public Vector2 WindowSize { get; private set; }
        private readonly string _viewName;
        private readonly string _surfaceViewId;
        protected ImGuiWindowFlags WindowFlags { get; set; } = ImGuiWindowFlags.NoScrollbar |
                                                               ImGuiWindowFlags.NoScrollWithMouse |
                                                               ImGuiWindowFlags.NoCollapse;

        private bool _canRenderWindow = false;

        private vec2 _contentRegionAvail;

        public EditorRenderSurfaceView(string viewName, RenderingSurface surface)
        {
            _surface = surface;
            Surface = surface;
            _viewName = viewName;
            _surfaceViewId = $"__SURFACE_VIEW__ID__{_viewName}";

        }

        public virtual void OnClose()
        {
        }

        public virtual void OnOpen()
        {
        }

        public virtual void OnUpdate()
        {
        }

        protected virtual void OnWindowRender()
        {

        }

        protected virtual vec2 GetViewSize()
        {
            return ImGui.GetContentRegionAvail().ToVec2();
        }
        protected virtual vec2 GetViewPosition()
        {
            return ImGui.GetCursorPos().ToVec2();
        }
        protected virtual vec2 GetPrevContentRegionAvail()
        {
            return _contentRegionAvail;
        }

        protected virtual void OnImguiWindowSizeChanged() { }
        protected virtual void OnRenderChildWindows() { }
        public virtual void OnDraw()
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, _canRenderWindow ? new Vector4(0.1f, 0.1f, 0.1f, 1.0f) : new Vector4(0, 0, 0, 1));
            ImGui.PushID(_surfaceViewId);
            ImGui.Begin(_viewName, WindowFlags);
            OnRenderChildWindows();

            var prevWinSize = WindowSize;
            WindowSize = ImGui.GetWindowSize();
            _contentRegionAvail = ImGui.GetContentRegionAvail().ToVec2();

            if (Mathf.RoundToInt(prevWinSize.ToVec2()) != Mathf.RoundToInt(WindowSize.ToVec2()))
            {
                OnImguiWindowSizeChanged();
            }

            var pos = ImGui.GetWindowPos();
            var imageCursorPos = GetViewPosition().ToVector2();
            var imageSize = GetViewSize().ToVector2();
            WindowPosition = pos;
            WindowPositionRender = pos + imageCursorPos;

            
            ICamera camera = null;
            var surfaceCamerasInUse = _surface.Cameras != null && _surface.Cameras.Length > 0 &&
                                      (_surface.Cameras?[0]?.TryGetTarget(out camera) ?? false) && camera != null &&
                                      camera.IsAlive && camera.IsEnabled;
            if (surfaceCamerasInUse)
            {
                var cameraRenderTarget = camera.OutRenderTexture?.NativeResource;

                if (cameraRenderTarget == null && _surface.RenderTextures != null && _surface.RenderTextures.Length > 0)
                {
                    cameraRenderTarget = _surface.RenderTextures[0].NativeResource;
                }

                if (cameraRenderTarget != null)
                {
                    _canRenderWindow = true;
                    var frameBuffer = cameraRenderTarget as GLFrameBuffer;
                    ImGui.SetCursorPos(imageCursorPos);
                    ImGui.Image((nint)frameBuffer.ColorTexture.Handle, imageSize, new Vector2(0, 1), new Vector2(1, 0));
                }
                else
                {
                    DefaultNoCamerasFound();
                }
            }
            else
            {
                DefaultNoCamerasFound();
            }

            OnWindowRender();

            ImGui.End();
            ImGui.PopID();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar();
        }

        private void DefaultNoCamerasFound()
        {
            // TODO: move this to be calculated just once.
            string text = "No valid cameras were found/enabled.";
            Vector2 textSize = ImGui.CalcTextSize(text);
            Vector2 windowSize = ImGui.GetContentRegionAvail();
            _canRenderWindow = false;
            ImGui.SetCursorPos(new Vector2((windowSize.X - textSize.X) * 0.5f, (windowSize.Y - textSize.Y) * 0.5f));

            ImGui.Text(text);
        }
    }
}
