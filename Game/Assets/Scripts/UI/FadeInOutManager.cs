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
            PostProcessingStackInternal.Push(_fadePostProcessing);
            _fadePostProcessing.Color = Color.Black;
             _fadePostProcessing.Value = 1;
            _instance = this;
        }

        public void FadeIn(float speed, Action onComplete = null)
        {
            StartCoroutine(Fade(1, speed, onComplete));
        }

        public void FadeOut(float speed, Action onComplete = null)
        {
            StartCoroutine(Fade(0, speed, onComplete));
        }

        private IEnumerator Fade(float target, float speed, Action onComplete)
        {
            var val = _fadePostProcessing.Value;
            var t = 0.0f;
            while (t < 1.0f)
            {
                _fadePostProcessing.Value = Mathf.Lerp(val, target, t);
                t += Time.UnscaledDeltaTime * speed;

                yield return null;
            }

            _fadePostProcessing.Value = target;

            onComplete?.Invoke();
        }
    }
}