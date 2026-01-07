using Editor.Views;
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
    internal abstract class EditorRenderSurfaceView : IEditorWindow
    {
        private readonly RenderingSurface _surface;
        protected RenderingSurface Surface { get; }
        public Vector2 WindowPositionRender { get; private set; }
        public Vector2 WindowPosition { get; private set; }
        public Vector2 WindowSize { get; private set; }
        private readonly string _viewName;
        private readonly string _surfaceViewId;
        protected ImGuiWindowFlags WindowFlags { get; set; }
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

        public virtual void OnDraw()
        {
            ImGuiWindowFlags flags = ImGuiWindowFlags.NoScrollbar |
                                     ImGuiWindowFlags.NoScrollWithMouse |
                                     ImGuiWindowFlags.NoCollapse | WindowFlags;

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 1));
            ImGui.PushID(_surfaceViewId);
            ImGui.Begin(_viewName, flags);
            WindowSize = ImGui.GetWindowSize();
            var pos = ImGui.GetWindowPos();
            WindowPosition = pos;
            WindowPositionRender = new Vector2((int)pos.X, (int)pos.Y + (int)ImGui.GetFrameHeight() / 2);
            ImGui.GetMousePos();

            var surfaceCamerasInUse = (_surface.Cameras != null && _surface.Cameras.Length > 0);
            if (surfaceCamerasInUse)
            {
                if (_surface.Cameras[0].TryGetTarget(out var camera) && camera != null && camera.IsAlive)
                {
                    var cameraRenderTarget = camera.OutRenderTexture?.NativeResource;

                    if (cameraRenderTarget == null)
                    {
                        cameraRenderTarget = _surface.RenderTextures[0].NativeResource;
                    }

                    var frameBuffer = cameraRenderTarget as GLFrameBuffer;
                    ImGui.Image((nint)frameBuffer.ColorTexture.Handle, ImGui.GetContentRegionAvail(), new Vector2(0, 1), new Vector2(1, 0));
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

            ImGui.SetCursorPos(new Vector2((windowSize.X - textSize.X) * 0.5f, (windowSize.Y - textSize.Y) * 0.5f));

            ImGui.Text(text);
        }
    }
}
