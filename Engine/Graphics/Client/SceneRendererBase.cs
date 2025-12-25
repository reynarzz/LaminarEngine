using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    internal class DrawOverlayOptions
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    internal class RenderingSurface
    {
        public ICamera[] Cameras { get; set; }
        public List<SceneRendererBase> SceneRenderers { get; set; } = new();
        public bool PickCameraFromSceneGraph { get; set; }
        public RenderTexture RenderTexture { get; set; }
        public bool RenderPostProcessing { get; set; }
        public bool RenderDebug { get; set; }
        public bool BlitToScreen { get; set; }
        public bool RenderUI { get; set; }
        public mat4 UIViewProj;
    }

    internal abstract class SceneRendererBase
    {
        private bool _isPrepared = false;
        public void OnBegin()
        {

        }
        public void OnPrepare(List<Renderer2D> worldRenderers, List<Renderer2D> uiRenderers)
        {
            if (!_isPrepared)
            {
                OnPrepareRendering(worldRenderers, uiRenderers);
                _isPrepared = true;
            }
        }

        protected abstract void OnPrepareRendering(List<Renderer2D> worldRenderers, List<Renderer2D> uiRenderers);
        public abstract RenderTexture OnRenderScene(RenderingSurface surface, ICamera camera, RenderTexture targetRenderTexture);
        public void OnEnd()
        {
            _isPrepared = false;
        }
    }
}