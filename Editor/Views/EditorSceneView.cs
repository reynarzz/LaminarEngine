using Editor.Rendering;
using Engine;
using Engine.Graphics;
using Engine.Graphics.OpenGL;
using Engine.GUI;
using Engine.Layers;
using GlmNet;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class EditorSceneView : EditorRenderSurfaceView
    {
        private readonly EditorCamera _camera;
        private Vector2 _screenSize;
        private mat4 _uiProj;
        internal bool IsMouseClicked { get; private set; }
        private readonly MousePickerSceneRenderer _mousePickerRenderer;
        public EditorSceneView(string viewName, RenderingSurface surface, EditorCamera camera) : base(viewName, surface)
        {
            _camera = camera;
            RenderingLayer.OnRenderingEnd += OnRenderingEnd;
            _mousePickerRenderer = new MousePickerSceneRenderer();
        }

        private void OnRenderingEnd()
        {
            if (IsMouseClicked && IsMouseInsideWindow())
            {
                SelecteObject();
                Surface.SceneRenderers.Remove(_mousePickerRenderer);
            }

            IsMouseClicked = false;
        }

        public override void OnUpdate()
        {

        }
        protected override void OnWindowRender()
        {
            _camera.Update();

            var size = ImGui.GetWindowSize();
            if ((int)_screenSize.X != (int)size.X || (int)_screenSize.Y != (int)size.Y)
            {
                _screenSize.X = (int)size.X;
                _screenSize.Y = (int)size.Y;

                mat4 FlipY = mat4.identity();
                FlipY[1] = new vec4(0, -1.0f, 0, 0);
                _uiProj = FlipY * _camera.Projection;

                //Surface.RenderTexture = new RenderTexture((int)_screenSize.X, (int)_screenSize.Y);
            }

            // Flip y movement
            var viewM = _camera.ViewMatrix;
            vec4 translation = viewM[3];
            translation = new vec4(translation.x, -translation.y, translation.z, translation.w);
            viewM[3] = translation;

            var viewTransformed = viewM * glm.translate(mat4.identity(), new vec3(0, -UICanvas.CanvasHeight));
            Surface.UIViewProj = _uiProj * viewTransformed;
            Surface.UIView = viewTransformed;
            Surface.UIProj = _uiProj;


            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && IsMouseInsideWindow())
            {
                IsMouseClicked = true;
                Surface.SceneRenderers.Add(_mousePickerRenderer);
            }
        }

        private bool IsMouseInsideWindow()
        {
            Vector2 mousePos = ImGui.GetMousePos();

            Vector2 windowPos = WindowPosition;
            Vector2 windowSize = WindowSize;

            float titleBarHeight = ImGui.GetFrameHeightWithSpacing();

            Vector2 mouseInContent = mousePos - new Vector2(windowPos.X, windowPos.Y + titleBarHeight);

            Vector2 contentSize = new Vector2(windowSize.X, windowSize.Y - titleBarHeight);

            return !(mouseInContent.X < 0 || mouseInContent.X >= contentSize.X ||
                   mouseInContent.Y < 0 || mouseInContent.Y >= contentSize.Y);
        }

        private void SelecteObject()
        {
            Vector2 mousePos = ImGui.GetMousePos();

            Vector2 windowPos = WindowPosition;
            Vector2 windowSize = WindowSize;

            float titleBarHeight = ImGui.GetFrameHeightWithSpacing();

            Vector2 mouseInContent = mousePos - new Vector2(windowPos.X, windowPos.Y + titleBarHeight);

            Vector2 contentSize = new Vector2(windowSize.X, windowSize.Y - titleBarHeight);

            if (!IsMouseInsideWindow())
                return;

            int rtWidth = Surface.RenderTextures[0].Width;
            int rtHeight = Surface.RenderTextures[0].Height;

            float scaleX = (float)rtWidth / contentSize.X;
            float scaleY = (float)rtHeight / contentSize.Y;

            int x = Mathf.Clamp(Mathf.RoundToInt(mouseInContent.X * scaleX), 0, rtWidth - 1);
            int y = Mathf.Clamp(rtHeight - 1 - Mathf.RoundToInt(mouseInContent.Y * scaleY), 0, rtHeight - 1) - 1;

            var colors = GfxDeviceManager.Current.ReadRenderTargetColors(Surface.RenderTextures[1].NativeResource, x, y, 1, 1);

            uint colorid = (ColorPacketRGBA)new Color32(colors[0], colors[1], colors[2], colors[3]);
            if (_mousePickerRenderer.RenderersIDs.TryGetValue(colorid, out var renderer))
            {
                Selector.Selected = renderer.Actor;
            }
            else
            {
                Selector.Selected = null;
            }
        }
    }
}