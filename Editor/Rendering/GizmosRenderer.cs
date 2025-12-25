using Engine;
using Engine.Graphics.Client;
using Engine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Rendering
{
    internal class GizmosRenderer : IGizmosRenderer
    {
        private readonly Batcher2D _batcher;
        public GizmosRenderer()
        {
            _batcher = new Batcher2D(20000);
        }

        public void OnBegin()
        {

        }

        public RenderTexture OnRender(ICamera camera, RenderTexture target)
        {
            return target;
        }

        public void OnEnd()
        {
        }
    }
}
