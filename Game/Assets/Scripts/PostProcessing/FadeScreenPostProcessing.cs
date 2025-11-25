using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class FadeScreenPostProcessing : PostProcessingPass
    {
        private Shader _shader;
        private RenderTexture _screenTex;
        private UniformValue[] _uniforms = new UniformValue[2];

        public FadeScreenPostProcessing()
        {
            _screenTex = new RenderTexture(1, 1);
            _shader = new Shader(Assets.GetText("Shaders/ScreenVert.vert").Text, Assets.GetText("Shaders/Fade.frag").Text);
            SetColor(Color.Black);
            SetAmount(1);
        }

        protected override RenderTexture Render(RenderTexture inRenderTexture)
        {
            if(inRenderTexture.Width != _screenTex.Width || inRenderTexture.Height != _screenTex.Height)
            {
                _screenTex.UpdateTarget(inRenderTexture.Width, inRenderTexture.Height);
            }

            Draw(_shader, inRenderTexture, _screenTex, _uniforms);
            return _screenTex;
        }

        public void SetColor(Color color)
        {
            _uniforms[1].SetVec3("Color", color);
        }

        public void SetAmount(float value)
        {
            _uniforms[0].SetFloat("Amount", value);
        }
    }
}