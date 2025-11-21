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
        private UIText _textPresents;
        public override void OnAwake()
        {
            _camera = new Actor<Camera>("Camera").GetComponent<Camera>();
            _defaultFont = Assets.Get<FontAsset>("Fonts/windows-bold[1].ttf");
            _camera.BackgroundColor = Color.Black;
            //CreateLaunchScreen();

            OnComplete();
        }

        private void CreateLaunchScreen()
        {
            var canvas = new Actor("Canvas").AddComponent<UICanvas>();

            var panel = new Actor("Panel").AddComponent<UIElement>();
            panel.Material = MaterialUtils.SpriteMaterial;
            panel.Transform.Parent = canvas.Transform;
            panel.Transform.LocalPosition = new vec3(canvas.RectTransform.Size.x / 2, canvas.RectTransform.Size.y / 2);
            panel.RectTransform.Size = canvas.RectTransform.Size;
            panel.RectTransform.Pivot = vec2.Half;

            //var logo = new Actor("Logo Image").AddComponent<UIImage>();
            //logo.Material = MaterialUtils.UIMaterial;
            //logo.Transform.Parent = panel.Transform;

            _textLabel = GetText("Text label", panel.Transform);
            _textLabel.FontSize = 150;
            _textLabel.SetText("Reynarz");
            _textLabel.Color = Color.Transparent;
            _textLabel.Transform.LocalPosition = new vec3(0, -50);
            _textPresents = GetText("subtitlePresents", panel.Transform);
            _textPresents.Color = Color.Transparent;
            _textPresents.FontSize = 50;
            _textPresents.Transform.LocalPosition = new vec3(0, 50, 0);
            _textPresents.SetText("Presents");
            StartCoroutine(WriteTitle());
        }

        private UIText GetText(string name, Transform parent)
        {
            var text = new Actor("Text label").AddComponent<UIText>();
            text.Material = MaterialUtils.SpriteMaterial;
            text.Font = _defaultFont;
            text.RectTransform.Pivot = vec2.Half;

            text.Transform.Parent = parent;
            text.Transform.LocalPosition = default;

            return text;
        }
        private IEnumerator WriteTitle()
        {
            IEnumerator Lerp(Color to, params Renderer2D[] renderer)
            {
                float tLerp = 0;
                var startColor = renderer.Select(x => x.Color).ToList();
                while (tLerp < 1.0f)
                {
                    tLerp += Time.DeltaTime;
                    for (int i = 0; i < startColor.Count; i++)
                    {
                        renderer[i].Color = Color.Lerp(startColor[i], to, tLerp);
                    }
                    yield return 0;
                }
            }

            IEnumerator LerpCam(Color to, Camera camera)
            {
                float tLerp = 0;
                var startColor = camera.BackgroundColor;
                while (tLerp < 1.0f)
                {
                    tLerp += Time.DeltaTime;
                    camera.BackgroundColor = Color.Lerp(startColor, to, tLerp);
                    yield return 0;
                }
            }
            yield return StartCoroutine(LerpCam(Color32.RGB(35), _camera));
            yield return new WaitForSeconds(0.2f);
            yield return StartCoroutine(Lerp(Color.White, _textLabel));
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(Lerp(Color.White, _textPresents));
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(Lerp(Color.Transparent, _textLabel, _textPresents));
            yield return StartCoroutine(LerpCam(Color.Black, _camera));


            OnComplete();
        }

        private void OnComplete()
        {
            SceneManager.Test_LoadScene(new Scene());
            new Actor<GameManager>("GameManager");
        }
    }
}
