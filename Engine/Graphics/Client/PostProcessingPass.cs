using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    public abstract class PostProcessingPass : IDisposable
    {
        private Action<Shader, RenderTexture, RenderTexture, UniformValue[]> _drawCallback;

        internal RenderTexture Render(RenderTexture inRenderTexture, Action<Shader, RenderTexture, RenderTexture, UniformValue[]> draw)
        {
            _drawCallback = draw;
            return Render(inRenderTexture);
        }

        protected abstract RenderTexture Render(RenderTexture inRenderTexture);

        protected void Draw(Shader shader, RenderTexture readFrom, RenderTexture applyTo)
        {
            Draw(shader, readFrom, applyTo, null);
        }
        protected void Draw(Shader shader, RenderTexture readFrom, RenderTexture applyTo, UniformValue[] uniforms)
        {
            _drawCallback(shader, readFrom, applyTo, uniforms);
        }

        public virtual void Dispose()
        {
            _drawCallback = null;
        }
    }
}
