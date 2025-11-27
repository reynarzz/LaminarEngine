using Engine;
using Engine.Graphics;
using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class FadeInOutManager : GameUI
    {
        private FadeScreenPostProcessing _fadePostProcessing = new();
        private static FadeInOutManager _instance;
        public static FadeInOutManager Instance => _instance;

        protected override void OnAwake()
        {
            PostProcessingStack.Push(_fadePostProcessing);
            _instance = this;
        }

        public void FadeIn(float speed)
        {
            StartCoroutine(Fade(1, speed));
        }

        public void FadeOut(float speed)
        {
            StartCoroutine(Fade(0, speed));
        }

        private IEnumerator Fade(float target, float speed)
        {
            var val = _fadePostProcessing.Value;
            while (_fadePostProcessing.Value != target)
            {
                _fadePostProcessing.Value = Mathf.Lerp(val, target, Time.UnscaledDeltaTime * speed);
                yield return null;
            }
        }
    }
}