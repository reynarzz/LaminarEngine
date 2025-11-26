using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Graphics
{
    public class PostProcessingSinglePass : PostProcessingPass
    {
        private readonly Shader _shader;
        private readonly RenderTexture _renderTextureOut;
        public UniformValue[] _uniforms = new UniformValue[10];
        private readonly Dictionary<string, int> _uniformIndex = new();
        public PostProcessingSinglePass(Shader shader)
        {
            _shader = shader;
            _renderTextureOut = new RenderTexture(Window.Width, Window.Height);
            Window.OnWindowChanged += UpdateRenderTargetSize;
        }

        private struct ListLayout<T>
        {
            public object? _syncRoot;
            public T[] Items;
            public int Size;
        }

        public void UpdateRenderTargetSize(int width, int height)
        {
            _renderTextureOut.UpdateTarget(width, height);
        }

        protected override RenderTexture Render(RenderTexture inRenderTexture)
        {
            Draw(_shader, inRenderTexture, _renderTextureOut, _uniforms);

            return _renderTextureOut;
        }

        public void SetValue(string name, float value)
        {
            if(!_uniformIndex.TryGetValue(name, out var index))
            {
                index = _uniformIndex.Count;
                _uniformIndex.Add(name, index);
            }

            _uniforms[index].SetFloat(name, value);
        }

        public override void Dispose()
        {
            base.Dispose();
            Window.OnWindowChanged -= UpdateRenderTargetSize;
            _renderTextureOut.OnDestroy();
        }
    }
}
