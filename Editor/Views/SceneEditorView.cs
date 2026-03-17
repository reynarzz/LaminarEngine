using Editor.Rendering;
using Engine;
using Engine.Graphics;
using Engine.Graphics.OpenGL;
using Engine.GUI;
using Engine.Layers;
using Engine.Utils;
using GlmNet;
using ImGuiNET;
using ImGuizmoNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class SceneEditorView : EditorRenderSurfaceView
    {
        private readonly EditorCamera _camera;
        private Vector2 _screenSize;
        private mat4 _uiProj;
        internal bool IsMouseClicked { get; private set; }
        private readonly MousePickerSceneRenderer _mousePickerRenderer;
        private vec2 _mouseFirstPickedPosition;
        private const float _maxPickedMouseDistance = 1.5f;

        public SceneEditorView(string viewName, RenderingSurface surface, EditorCamera camera) : base(viewName, "Window/Scene", surface)
        {
            _camera = camera;
            RenderingLayer.OnRenderingEnd += OnRenderingEnd;
            _mousePickerRenderer = new MousePickerSceneRenderer();
            _mousePickerRenderer.RenderTextureIndex = 1;

            ImGuizmo.SetGizmosPixelSize(60);
        }

        private void OnRenderingEnd()
        {
            if (IsMouseClicked)
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

        private void RenderGuizmo()
        {
            ImGuizmo.Enable(true);

            var offset = ImGui.GetFrameHeight();// + ImGui.GetStyle().FrameBorderSize;

            var windowAdjustSize = new vec2(WindowSize.X, WindowSize.Y - offset);


            bool _isDefaultGizmosEnabled = true;
            var gizmoPos = ImGui.GetCursorScreenPos();
            var gizmoSize = WindowSize;
            if ((ImGuizmo.IsOver() || ImGuizmo.IsUsing()))
            {
                WindowFlags |= ImGuiWindowFlags.NoMove;
            }
            else
            {
                WindowFlags &= ~ImGuiWindowFlags.NoMove;
            }
            ImGuizmo.SetRect(WindowPosition.X, WindowPosition.Y + offset, windowAdjustSize.x, windowAdjustSize.y);
            ImGuizmo.BeginFrame();

            ImGuizmo.SetDrawlist(ImGui.GetWindowDrawList());
            ImGuizmo.AllowAxisFlip(false);
            ImGuizmo.SetOrthographic(false);

            mat4 view = _camera.ViewMatrix; // TODO: Change to ui matrix if a UI component is selected.
            mat4 projection = _camera.Projection;

            float titleBarHeight = ImGui.GetFrameHeightWithSpacing();
            float windowPadding = ImGui.GetStyle().WindowPadding.X;

            ImGui.SetCursorScreenPos(ImGui.GetItemRectMin());

            if (Selector.Transform)
            {
                var selectedTransform = Selector.Transform;

                mat4 model = selectedTransform.GetRenderingWorldMatrix();
                mat4 delta = default;
                OPERATION operation = OPERATION.TRANSLATE;
                MODE mode = MODE.LOCAL;




                // This prevents the guizmo from disappearing when any scale scale component is zero.
                const float minValue = 1.0f;
                var s = model[0];
                if (operation != OPERATION.SCALE)
                {
                    if (Mathf.IsAlmostZero(s.x))
                    {
                        s.x = minValue;
                        model[0] = s;
                    }
                    s = model[1];
                    if (Mathf.IsAlmostZero(s.y))
                    {
                        s.y = minValue;
                        model[1] = s;
                    }
                    s = model[2];
                    if (Mathf.IsAlmostZero(s.z))
                    {
                        s.z = minValue;
                        model[2] = s;
                    }
                }

                if (ImGuizmo.Manipulate(ref view.c0.x, ref projection.c0.x, operation, mode, ref model.c0.x, ref delta.c0.x))
                {
                    if (ImGui.IsWindowHovered())
                    {
                        vec3 position = default;
                        vec3 rotation = default;
                        vec3 scale = default;
                        vec3 deltaPosition = default;
                        vec3 deltaRotation = default;
                        vec3 deltaScale = default;

                        ImGuizmo.DecomposeMatrixToComponents(ref model.c0.x, ref position.x, ref rotation.x, ref scale.x);
                        ImGuizmo.DecomposeMatrixToComponents(ref delta.c0.x, ref deltaPosition.x, ref deltaRotation.x, ref deltaScale.x);

                        SetTransform(operation, mode, selectedTransform, position, rotation, scale,
                                     deltaPosition, deltaRotation, deltaScale);
                    }
                    EditorSystem.Save.MarkDirty(selectedTransform);
                }
            }

            ImGuizmo.ViewManipulate(ref view.c0.x, 30, new Vector2(WindowPosition.X + WindowSize.X - 90, WindowPosition.Y + 10), new Vector2(100, 100), 0);
        }

        private void SetTransform(OPERATION operation, MODE mode, Transform selectedTransform,
            vec3 position, vec3 rotation, vec3 scale,
            vec3 deltaPosition, vec3 deltaRotation, vec3 deltaScale)
        {
            switch (operation)
            {
                case OPERATION.TRANSLATE_X:
                    break;
                case OPERATION.TRANSLATE_Y:
                    break;
                case OPERATION.TRANSLATE_Z:
                    break;
                case OPERATION.ROTATE_X:
                    break;
                case OPERATION.ROTATE_Y:
                    break;
                case OPERATION.ROTATE_Z:
                    break;
                case OPERATION.ROTATE_SCREEN:
                    break;
                case OPERATION.SCALE_X:
                    break;
                case OPERATION.SCALE_Y:
                    break;
                case OPERATION.SCALE_Z:
                    break;
                case OPERATION.BOUNDS:
                    break;
                case OPERATION.TRANSLATE:
                    selectedTransform.WorldPosition += deltaPosition;
                    break;
                case OPERATION.ROTATE:
                    selectedTransform.WorldEulerAngles = rotation;
                    break;
                case OPERATION.SCALE:
                    selectedTransform.WorldScale = scale;
                    break;
                default:
                    break;
            }
        }


        protected override void OnWindowRender()
        {
            _camera.Update();
            RenderGuizmo();

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

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left) && IsMouseInsideWindow() && ImGui.IsWindowHovered())
            {
                var mousePickedDist = (_mouseFirstPickedPosition - ImGui.GetMousePos().ToVec2()).Magnitude;

                if (mousePickedDist > _maxPickedMouseDistance)
                {
                    _mousePickerRenderer.ClearPickedList();
                }
                IsMouseClicked = true;

                // Adds the mouse picker scene renderer to the queue so it will be rendered next frame.
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

            var isInsideRect = !(mouseInContent.X < 0 || mouseInContent.X >= contentSize.X ||
                                 mouseInContent.Y < 0 || mouseInContent.Y >= contentSize.Y);

            return isInsideRect && !ImGuizmo.IsUsing();
        }
        private RendererData2D GetMousePickedRenderer(IReadOnlyDictionary<uint, RendererData2D> colorIds, RenderTexture renderTexture)
        {
            Vector2 mousePos = ImGui.GetMousePos();

            Vector2 windowPos = WindowPosition;
            Vector2 windowSize = WindowSize;

            float titleBarHeight = ImGui.GetFrameHeightWithSpacing();

            Vector2 mouseInContent = mousePos - new Vector2(windowPos.X, windowPos.Y + titleBarHeight);

            Vector2 contentSize = new Vector2(windowSize.X, windowSize.Y - titleBarHeight);

            if (!IsMouseInsideWindow())
                return null;

            int rtWidth = renderTexture.Width;
            int rtHeight = renderTexture.Height;

            float scaleX = (float)rtWidth / contentSize.X;
            float scaleY = (float)rtHeight / contentSize.Y;

            int x = Mathf.Clamp(Mathf.RoundToInt(mouseInContent.X * scaleX), 0, rtWidth - 1);
            int y = Mathf.Clamp(rtHeight - 1 - Mathf.RoundToInt(mouseInContent.Y * scaleY), 0, rtHeight - 1) - 1;

            var colors = GfxDeviceManager.Current.ReadRenderTargetColors(renderTexture.NativeResource, x, y, 1, 1);

            uint colorid = (ColorPacketRGBA)new Color32(colors[0], colors[1], colors[2], colors[3]);
            if (colorIds.TryGetValue(colorid, out var renderer))
            {
                return renderer;
            }

            return null;
        }
        private void SelecteObject()
        {
            var renderer = GetMousePickedRenderer(_mousePickerRenderer.RenderersIDs, Surface.RenderTextures[1]);
            if (renderer != null)
            {
                Selector.Selected = renderer.Transform.Actor;

                if (_mousePickerRenderer.PickedRenderersCount == 0)
                {
                    _mouseFirstPickedPosition = ImGui.GetMousePos().ToVec2();
                }

                _mousePickerRenderer.OnPickRenderer(renderer.GetID());
            }
            else
            {
                var pickedCount = _mousePickerRenderer.PickedRenderersCount;
                _mousePickerRenderer.ClearPickedList();

                if (pickedCount != 0)
                {
                    // Use back buffer to check if the topmost renderer is still there.
                    renderer = GetMousePickedRenderer(_mousePickerRenderer.RenderersIDsBackBuffer, _mousePickerRenderer.PickedBackBuffer);
                    if (renderer != null)
                    {
                        _mousePickerRenderer.OnPickRenderer(renderer.GetID());
                    }

                    Selector.Selected = renderer?.Transform?.Actor;
                }
                else
                {
                    Selector.Selected = null;
                }
            }
        }
    }
}