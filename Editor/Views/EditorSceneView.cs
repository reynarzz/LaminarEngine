using Editor.Rendering;
using Engine;
using Engine.Graphics;
using Engine.Graphics.OpenGL;
using Engine.GUI;
using Engine.Layers;
using Engine.Utils;
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
        private RendererData2D _firstPickedRenderer;
        private RendererData2D _lastPickedRenderer;
        internal bool IsMouseClicked { get; private set; }
        private readonly MousePickerSceneRenderer _mousePickerRenderer;
        private vec2 _mouseFirstPickedPosition;
        private const float _maxPickedMouseDistance = 1.5f;

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

                // No need to keep rendering the id for objects since the mouse click finished.
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
                var mousePickedDist = (_mouseFirstPickedPosition - ImGui.GetMousePos().ToVec2()).Magnitude;

                if (mousePickedDist > _maxPickedMouseDistance)
                {
                    _mousePickerRenderer.ClearPickedList();
                }
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
            float windowPadding = ImGui.GetStyle().WindowPadding.X;

            // Offset mouse into content space, accounting for grab padding
            Vector2 mouseInContent = mousePos - new Vector2(
                windowPos.X + windowPadding,
                windowPos.Y + titleBarHeight + windowPadding
            );

            // Shrink content size by grab padding on both sides
            Vector2 contentSize = new Vector2(
                windowSize.X - windowPadding * 2.0f,
                windowSize.Y - titleBarHeight - windowPadding * 2.0f
            );

            return !(mouseInContent.X < 0 || mouseInContent.X >= contentSize.X ||
                     mouseInContent.Y < 0 || mouseInContent.Y >= contentSize.Y);
        }
        private RendererData2D GetMousePickedRenderer()
        {
            Vector2 mousePos = ImGui.GetMousePos();

            Vector2 windowPos = WindowPosition;
            Vector2 windowSize = WindowSize;

            float titleBarHeight = ImGui.GetFrameHeightWithSpacing();

            Vector2 mouseInContent = mousePos - new Vector2(windowPos.X, windowPos.Y + titleBarHeight);

            Vector2 contentSize = new Vector2(windowSize.X, windowSize.Y - titleBarHeight);

            if (!IsMouseInsideWindow())
                return null;

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
                return renderer;
            }

            return null;
        }
        private void SelecteObject()
        {
            var renderer = GetMousePickedRenderer();
            if (renderer != null)
            {
                Selector.Selected = renderer.Transform.Actor;

                if (_mousePickerRenderer.PickedRenderersCount == 0)
                {
                    _mouseFirstPickedPosition = ImGui.GetMousePos().ToVec2();
                    _firstPickedRenderer = renderer;
                }

                _lastPickedRenderer = renderer;
                _mousePickerRenderer.OnPickRenderer(renderer.GetID());
                // Debug.Log(renderer.Transform.Name);
            }
            else
            {
                if (_mousePickerRenderer.PickedRenderersCount != 0)
                {
                    renderer = _firstPickedRenderer;

                    _mousePickerRenderer.ClearPickedList();

                    if (renderer != null && renderer.Transform)
                    {
                        var mousePickedDist = (_mouseFirstPickedPosition - ImGui.GetMousePos().ToVec2()).Magnitude;
                        if (mousePickedDist > _maxPickedMouseDistance && _lastPickedRenderer != null)
                        {
                            renderer = _lastPickedRenderer;
                            Debug.Log("Last: " + _lastPickedRenderer.Transform.Name);
                        }

                        _lastPickedRenderer = null;

                        _mousePickerRenderer.OnPickRenderer(renderer.GetID());
                        Selector.Selected = renderer?.Transform?.Actor;
                    }
                    else
                    {
                        Selector.Selected = null;
                    }
                }
                else
                {
                    Selector.Selected = null;
                }
            }
        }
    }
}