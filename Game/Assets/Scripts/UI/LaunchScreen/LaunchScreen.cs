using Engine;
using Engine.GUI;
using Engine.Utils;
using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class LaunchScreen : ScriptBehavior
    {
        private Camera _camera;
        private FontAsset _defaultFont;
        private UIText _textLabel;
        public override void OnAwake()
        {
            _camera = new Actor<Camera>("Camera").GetComponent<Camera>();
            //
            _defaultFont = Assets.Get<FontAsset>("Fonts/windows-bold[1].ttf");
            _camera.BackgroundColor = new Color32(28, 28, 28, 255);
            CreateLaunchScreen();
        }

        private void CreateLaunchScreen()
        {
            var canvas = new Actor("Canvas").AddComponent<UICanvas>();

            var panel = new Actor("Panel").AddComponent<UIElement>();
            panel.Transform.Parent = canvas.Transform;
            panel.Transform.LocalPosition = new vec3(canvas.RectTransform.Size.x / 2, canvas.RectTransform.Size.y / 2);
            panel.RectTransform.Size = canvas.RectTransform.Size;
            panel.RectTransform.Pivot = vec2.Half;

            //var logo = new Actor("Logo Image").AddComponent<UIImage>();
            //logo.Material = MaterialUtils.UIMaterial;
            //logo.Transform.Parent = panel.Transform;

            _textLabel = new Actor("Text label").AddComponent<UIText>();
            _textLabel.Material = MaterialUtils.SpriteMaterial;
            _textLabel.Font = _defaultFont;
            _textLabel.RectTransform.Pivot = vec2.Half;

            _textLabel.Transform.Parent = panel.Transform;
            _textLabel.Transform.LocalPosition = default;
            _textLabel.FontSize = 150;
            _textLabel.SetText("");

            StartCoroutine(WriteTitle("Reynarz"));
        }

        private IEnumerator WriteTitle(string text)
        {
            //var textWait = new WaitForSeconds(0.1f);
            //for (int i = 0; i < test.Length; i++)
            //{
            //    _textLabel.Append(test[i]);
            //    yield return textWait;
            //}

            IEnumerator Lerp(Renderer2D renderer, Color to)
            {
                float tLerp = 0;
                var startColor = renderer.Color;
                while (tLerp < 1.0f)
                {
                    tLerp += Time.DeltaTime;
                    renderer.Color = Color.Lerp(startColor, to, tLerp);
                    yield return 0;
                }
            }

            _textLabel.Color = Color.Transparent;
            _textLabel.SetText(text);

            yield return StartCoroutine(Lerp(_textLabel, Color.White));
            
            yield return new WaitForSeconds(1);

            yield return StartCoroutine(Lerp(_textLabel, Color.Transparent));

            OnComplete();
        }

        private void OnComplete()
        {
            SceneManager.Test_LoadScene(new Scene());
            new Actor<GameManager>("GameManager");
        }
    }
}
