using Engine;
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
    internal class EditorSceneView : EditorRenderSurfaceView
    {
        private readonly EditorCamera _camera;
        private Vector2 _screenSize;

        public EditorSceneView(string viewName, RenderingLayer.RenderingSurface surface, EditorCamera camera) : base(viewName, surface)
        {
            _camera = camera;
        }

        protected override void OnWindowRender()
        {
            _camera.Update();

            var size = ImGui.GetWindowSize();

            //if ((int)_screenSize.X != (int)size.X || (int)_screenSize.Y != (int)size.Y)
            //{
            //    _screenSize.X = (int)size.X;
            //    _screenSize.Y = (int)size.Y;

            //   Surface.RenderTexture = new RenderTexture((int)_screenSize.X, (int)_screenSize.Y);
            //}
        }
    }
}
