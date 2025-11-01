using Engine;
using Engine.GUI;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PauseMenu : ScriptBehavior
    {
        private UIImage _background;
        private UIImage _buttonImage;
        private UIText _buttonText;
        private Button _button;

        private UICanvas _canvas;
        private UIText _titleText;
        private bool _show = false;

        private List<UIGraphicsElement> _graphics = new();
        public override void OnStart()
        {
            _canvas = new Actor("Pause menu Canvas").AddComponent<UICanvas>();
            _canvas.Transform.Parent = Transform;

            var backSize = new vec2(512 * 2, 288 * 2);
            // Background
            _background = NewImage("Background", backSize * 0.5f, backSize, Color.White, _canvas.Transform);
            _background.BlockEvents = true;
            _background.ReceiveEvents = true;
            _background.Material = new Material(new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/Blur.frag").Text));
            _background.Material.Passes[0].IsScreenGrabPass = true;

            // Title text
            _titleText = NewText("Title text", "Pause", new vec2(0, -110), _background.Transform);
            _titleText.FontSize = 70;

            var resumeButtonImage = NewImage("Resume button image", new vec2(0, 100), new vec2(200, 40), Color.Coral, _background.Transform);
            resumeButtonImage.AddComponent<Button>().OnButtonClick += OnResume;
            var text = NewText("Resume text", "Resume", default, resumeButtonImage.Transform);
            text.ReceiveEvents = false;
            text.RectTransform.Size.y = resumeButtonImage.RectTransform.Size.y;
            text.RectTransform.Size.x = resumeButtonImage.RectTransform.Size.x;
            // text.Wrap = TextWrap.WordWrap;
            text.Fit = TextFit.ShrinkToFit;
            text.Horizontal = TextHorizontalAlignment.Center;
            //text.Padding.Right = 10;
            //text.Padding.Left = 10;
            //Time.TimeScale = 0;

            _graphics.Add(text);
            _graphics.Add(_titleText);
            _graphics.Add(_background);
            _graphics.Add(resumeButtonImage);


            var horizontalLayout = new Actor<UIImage>("HorizontalRect").AddComponent<GridLayout>();
            horizontalLayout.Transform.Parent = _background.Transform;
            horizontalLayout.Transform.LocalPosition = new vec3(0, 0);
            horizontalLayout.ResizeToFitVertical = true;
            horizontalLayout.ResizeToFitHorizontal = true;
            horizontalLayout.Spacing = 10;
            horizontalLayout.Padding = new Thickness(10);
            horizontalLayout.StartPivot = new vec2(0.5f, 0.5f);
            horizontalLayout.MaxPerRow = 2;
            var img = horizontalLayout.GetComponent<UIImage>();
            img.Material = GameManager.DefaultMaterial;
            img.Color = Color.Gray;

            NewImage("Quad1", default, new vec2(100, 100), Color.Yellow, horizontalLayout.Transform);
            NewImage("Quad2", default, new vec2(100, 100), Color.Red, horizontalLayout.Transform);
            NewImage("Quad3", default, new vec2(100, 100), Color.Blue, horizontalLayout.Transform);
            NewImage("Quad4", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad4", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad4", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad4", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad4", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad4", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad4", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);

        }

        private void OnResume()
        {
            Debug.Log("Button down: ");
            Time.TimeScale = 1;
            //Actor.IsActiveSelf = false;
            _show = false;
        }

        public void OnPause()
        {
            //Actor.IsActiveSelf = !Actor.IsActiveSelf;
            _show = !_show;
            Time.TimeScale = _show ? 0 : 1;
        }

        public override void OnUpdate()
        {
            void SetColorAlpha(float alpha)
            {
                foreach (var item in _graphics)
                {
                    var color = item.Color;
                    color.A = alpha;
                    item.Color = Color.MoveTowards(item.Color, color, Time.UnscaledDeltaTime * 5f);
                }
            }
            if (_show)
            {
                SetColorAlpha(1);
            }
            else
            {
                SetColorAlpha(0);
            }
        }

        private UIText NewText(string name, string value, vec2 position, Transform parent)
        {
            var text = new Actor(name).AddComponent<UIText>();
            text.Transform.Parent = parent;
            text.Font = GameManager.DefaultFont;
            text.Material = GameManager.DefaultMaterial;
            text.SetText(value);
            text.Transform.LocalPosition = position;
            return text;
        }

        private UIImage NewImage(string name, vec2 position, vec2 size, Color color, Transform parent)
        {
            var image = new Actor(name).AddComponent<UIImage>();
            image.Material = GameManager.DefaultMaterial;
            image.Transform.Parent = parent;
            image.RectTransform.Pivot = vec2.Half;
            image.RectTransform.Size = size;
            image.Transform.LocalPosition = position;
            image.Color = color;

            return image;
        }
    }
}