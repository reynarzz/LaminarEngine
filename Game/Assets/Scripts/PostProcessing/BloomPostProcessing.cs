using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class BloomPostProcessing : PostProcessingPass
    {
        private Shader _brightPassShader;
        private Shader _blurPassShader;
        private Shader _combinePassShader;

        private RenderTexture _brightRenderTexture;
        private RenderTexture _combineRenderTexture;
        private RenderTexture _blurRenderTexture;
        private RenderTexture _blurRenderTexture2;
        private UniformValue[] _combinePassUniforms;
        private UniformValue[] _brightPassUniforms;
        private UniformValue[] _horizontalPassUniforms;
        private UniformValue[] _verticalPassUniforms;

        public BloomPostProcessing()
        {
            var vertex = Assets.GetText("Shaders/ScreenVert.vert").Text;

            _brightPassShader = new Shader(vertex, Assets.GetText("Shaders/Bloom/Bloom_BrightPass.frag").Text);
            _blurPassShader = new Shader(vertex, Assets.GetText("Shaders/Bloom/Bloom_GaussianBlur.frag").Text);
            _combinePassShader = new Shader(vertex, Assets.GetText("Shaders/Bloom/Bloom_Combine.frag").Text);

            _brightRenderTexture = new RenderTexture(Screen.Width / 3, Screen.Height / 3);
            _blurRenderTexture = new RenderTexture(_brightRenderTexture.Width, _brightRenderTexture.Height);
            _blurRenderTexture2 = new RenderTexture(_brightRenderTexture.Width, _brightRenderTexture.Height);
            _combineRenderTexture = new RenderTexture(Screen.Width, Screen.Height);

            _combinePassUniforms = 
            [
                UniformValue.AsRenderTexture("uBlurTex", _blurRenderTexture2),
                UniformValue.AsFloat("uBloomStrength", 0.2f)
            ];
            _brightPassUniforms =
            [
                UniformValue.AsFloat("uThreshold", 0.6f),
                UniformValue.AsFloat("uKnee", 0.2f),
            ];
            _horizontalPassUniforms =
            [
                UniformValue.AsInt("uHorizontal", 1)
            ];

            _verticalPassUniforms =
            [
                UniformValue.AsInt("uHorizontal", 0)
            ];

            WindowManager.Window.OnWindowChanged += Window_OnWindowChanged;
        }

        private void Window_OnWindowChanged(int width, int height)
        {
            _brightRenderTexture.UpdateTarget(width / 3, height / 3);
            _combineRenderTexture.UpdateTarget(width, height);
            _blurRenderTexture.UpdateTarget(_brightRenderTexture.Width, _brightRenderTexture.Height);
            _blurRenderTexture2.UpdateTarget(_brightRenderTexture.Width, _brightRenderTexture.Height);
        }

        protected override RenderTexture Render(RenderTexture inRenderTexture)
        {
            Draw(_brightPassShader, inRenderTexture, _brightRenderTexture, _brightPassUniforms);
            Draw(_blurPassShader, _brightRenderTexture, _blurRenderTexture, _horizontalPassUniforms);
            Draw(_blurPassShader, _blurRenderTexture, _blurRenderTexture2, _verticalPassUniforms);
            Draw(_combinePassShader, inRenderTexture, _combineRenderTexture, _combinePassUniforms);

            return _blurRenderTexture2;
        }

        public override void Dispose()
        {
            base.Dispose();
            // _brightPassShader.OnDestroy();
        }
    }
}
