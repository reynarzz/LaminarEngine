using Engine;
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
        public EditorSceneView(string viewName, RenderingLayer.RenderingSurface surface, EditorCamera camera) : base(viewName, surface)
        {
            _camera = camera;
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

            Surface.UIViewProj = _uiProj * viewM * glm.translate(mat4.identity(), new vec3(-UICanvas.CanvasWidth, -UICanvas.CanvasHeight));


         
        }
    }
}
