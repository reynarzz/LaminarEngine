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
            var index = GetIndex(name);

            if (index >= 0)
            {
                _uniforms[index].SetFloat(name, value);
            }
        }

        private int GetIndex(string name)
        {
            if (!_uniformIndex.TryGetValue(name, out var index))
            {
                index = _uniformIndex.Count;
                if (index >= _uniforms.Length)
                {
                    Debug.Error($"Max uniforms supported for post-processing are: '{_uniforms.Length}', can't add more.");
                    return -1;
                }
                _uniformIndex.Add(name, index);
            }

            return index;
        }

        public override void Dispose()
        {
            base.Dispose();
            Window.OnWindowChanged -= UpdateRenderTargetSize;
            _renderTextureOut.OnDestroy();
        }
    }
}
