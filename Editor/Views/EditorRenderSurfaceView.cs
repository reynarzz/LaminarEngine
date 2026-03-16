using Editor.Utils;
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
    internal abstract class EditorRenderSurfaceView : EditorWindow
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

        public EditorRenderSurfaceView(string viewName, string menuItemPath, RenderingSurface surface) : base(menuItemPath)
        {
            _surface = surface;
            Surface = surface;
            _viewName = viewName;
            _surfaceViewId = $"__SURFACE_VIEW__ID__{_viewName}";

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
        public override void OnDraw()
        {
            ImGui.PushStyleColor(ImGuiCol.WindowBg, _canRenderWindow ? new Vector4(0.1f, 0.1f, 0.1f, 1.0f) : new Vector4(0, 0, 0, 1));
            if (OnBeginWindow(_viewName, WindowFlags, true, new vec2()))
            {
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
                var imageSize = GetViewSize();
                WindowPosition = pos;
                WindowPositionRender = pos + imageCursorPos;


                ICamera camera = null;

                if (_surface.Cameras != null && _surface.Cameras.Length > 0)
                {
                    camera = _surface.Cameras[0];
                }

                var surfaceCamerasInUse = camera != null && camera.IsValid;
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
                        EditorImGui.Image((nint)frameBuffer.ColorTexture.Handle, imageSize);
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
            }
            OnEndWindow();
            ImGui.PopStyleColor();
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
