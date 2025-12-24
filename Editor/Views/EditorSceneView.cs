using Engine.Layers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    internal class EditorSceneView : EditorRenderSurfaceView
    {
        public EditorSceneView(string viewName, RenderingLayer.RenderingSurface surface) : base(viewName, surface)
        {
        }
    }
}
