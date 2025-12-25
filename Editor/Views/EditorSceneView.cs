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
        private Texture texture;
        private TextureDescriptor _textDesc;
        private byte[] _blackPixel = [0, 0, 0, 0xFF];
        public EditorSceneView(string viewName, RenderingSurface surface, EditorCamera camera) : base(viewName, surface)
        {
            _camera = camera;

            texture = new Texture2D(TextureMode.Clamp, 1, 1, 4, [0xFF, 0xFF, 0xFF, 0xFF]);

            _textDesc = new TextureDescriptor()
            {
                Width = 1,
                Height = 1,
                Channels = 1,
                Mode = TextureMode.Clamp,

            };
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

            Surface.UIViewProj = _uiProj * viewM * glm.translate(mat4.identity(), new vec3(0, -UICanvas.CanvasHeight));

            GetPixelMousePicker();
        }

        public override void OnRender()
        {
            base.OnRender();
            ImGui.Begin("Image testt");
            // Draw the image and get the region it occupies
            Vector2 imageSize = new Vector2(10, 10);
            ImGui.Image((nint)(texture.NativeResource as GLTexture).Handle, imageSize, new Vector2(0, 1), new Vector2(1, 0));

            ImGui.End();
        }

        private void GetPixelMousePicker()
        {
            Vector2 mousePos = ImGui.GetMousePos();

            Vector2 windowPos = WindowPosition;
            Vector2 windowSize = WindowSize;

            float titleBarHeight = ImGui.GetFrameHeightWithSpacing();

            Vector2 mouseInContent = mousePos - new Vector2(windowPos.X, windowPos.Y + titleBarHeight);

            Vector2 contentSize = new Vector2(windowSize.X, windowSize.Y - titleBarHeight);

            if (mouseInContent.X < 0 || mouseInContent.X >= contentSize.X ||
                mouseInContent.Y < 0 || mouseInContent.Y >= contentSize.Y)
            {
                _textDesc.Buffer = _blackPixel;
                GfxDeviceManager.Current.UpdateResouce(texture.NativeResource, _textDesc);
                return;
            }

            int rtWidth = Surface.RenderTexture.Width;
            int rtHeight = Surface.RenderTexture.Height;

            float scaleX = (float)rtWidth / contentSize.X;
            float scaleY = (float)rtHeight / contentSize.Y;

            int x = Mathf.Clamp(Mathf.RoundToInt(mouseInContent.X * scaleX), 0, rtWidth - 1);
            int y = Mathf.Clamp(rtHeight - 1 - Mathf.RoundToInt(mouseInContent.Y * scaleY), 0, rtHeight - 1);

            byte[] colors = GfxDeviceManager.Current.ReadRenderTargetColors(Surface.RenderTexture.NativeResource, x, y, 1, 1);

            _textDesc.Buffer = colors;
            GfxDeviceManager.Current.UpdateResouce(texture.NativeResource, _textDesc);
        }
    }
}