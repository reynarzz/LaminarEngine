using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics.Client
{
    internal interface IGizmosRenderer
    {
        void OnBegin(ICamera camera);
        RenderTexture OnRender(ICamera camera, RenderingSurface surface, RenderTexture target);
        void OnEnd();
    }
}
