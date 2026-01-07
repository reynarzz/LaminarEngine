using Engine.Graphics.Client;
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
        public WeakReference<ICamera>[] Cameras { get; set; }
        public List<SceneRendererBase> SceneRenderers { get; set; } = new();
        public bool PickCameraFromSceneGraph { get; set; }
        public RenderTexture[] RenderTextures { get; set; }
        public bool RenderPostProcessing { get; set; }
        public bool RenderDebug { get; set; }
        public bool BlitToScreen { get; set; }
        public bool RenderUI { get; set; }
        public IGizmosRenderer GizmosRenderer { get; set; }
        public bool DrawGizmos { get; set; }
        public mat4 UIViewProj;
        public mat4 UIView;
        public mat4 UIProj;
    }

    internal abstract class SceneRendererBase
    {
        private bool _isPrepared = false;
        public int RenderTextureIndex { get; set; }
        public void OnBegin()
        {

        }
        public void OnPrepare(List<RendererData2D> worldRenderers, List<RendererData2D> uiRenderers)
        {
            if (!_isPrepared)
            {
                OnPrepareRendering(worldRenderers, uiRenderers);
                _isPrepared = true;
            }
        }

        protected abstract void OnPrepareRendering(List<RendererData2D> worldRenderers, List<RendererData2D> uiRenderers);
        public abstract RenderTexture OnRenderScene(RenderingSurface surface, ICamera camera, RenderTexture targetRenderTexture);
        public void OnEnd()
        {
            _isPrepared = false;
            OnRenderingEnd();
        }

        protected virtual void OnRenderingEnd()
        {

        }
    }
}