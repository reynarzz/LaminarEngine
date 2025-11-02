using Engine;
using Engine.GUI;
using Engine.Utils;
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

            // LayoutTest();

            // Title text
            _titleText = NewText("Title text", "Pause", new vec2(0, -110), _background.Transform);
            _titleText.FontSize = 70;

            var resumeButtonImage = NewImage("Resume button image", new vec2(0, 100), new vec2(200, 40), Color.Gray, _background.Transform);
            resumeButtonImage.AddComponent<Button>().OnButtonClick += OnResume;
            resumeButtonImage.AddComponent<ContentSizeFitter>().Padding = new Thickness(10);
            // resumeButtonImage.AddComponent<Test_UIEvent>();

            var text = NewText("Resume text", "Resume", default, resumeButtonImage.Transform);
            text.ReceiveEvents = false;
            text.RectTransform.Size.y = resumeButtonImage.RectTransform.Size.y;
            //text.RectTransform.Size.x = resumeButtonImage.RectTransform.Size.x;
            // text.Wrap = TextWrap.WordWrap;
            text.Fit = TextFit.ExpandToFit;
            text.Horizontal = TextHorizontalAlignment.Center;
            //text.Padding.Right = 10;
            //text.Padding.Left = 10;
            //Time.TimeScale = 0;
            //text.AddComponent<Test_UIEvent>();

            _graphics.Add(text);
            _graphics.Add(_titleText);
            _graphics.Add(_background);
            _graphics.Add(resumeButtonImage);

            LogRecursive(_background.Transform);
        }

        private void LayoutTest()
        {
            var horizontalLayout = new Actor<UIImage>("HorizontalRect").AddComponent<GridLayout>();
            horizontalLayout.Transform.Parent = _background.Transform;
            horizontalLayout.Transform.LocalPosition = new vec3(0, 0);
            horizontalLayout.ResizeToFitVertical = true;
            horizontalLayout.ResizeToFitHorizontal = true;
            horizontalLayout.Spacing = 10;
            horizontalLayout.Padding = new Thickness(10);
            horizontalLayout.StartPivot = new vec2(0.5f, 0.5f);
            horizontalLayout.MaxPerRow = 3;
            var img = horizontalLayout.GetComponent<UIImage>();
            img.Material = _background.Material; // GameManager.DefaultMaterial;
            img.Color = Color.Gray;

            var parent = NewImage("Quad1", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            var tex = Assets.GetTexture("starkTileset.png");
            var coins = TextureAtlasUtils.SliceSprites(tex, 16, 16, 281, 4);

            var iconContent = NewImage("Content", default, new vec2(45, 45), Color.White, parent.Transform);
            iconContent.RectTransform.Pivot = new vec2(0.5f, 0.55f);
            var animator = iconContent.AddComponent<Animator>();
            var clip = new AnimationClip("Sprite");
            var spriteCurve = new SpriteCurve();
            for (int i = 0; i < coins.Length; i++)
            {
                spriteCurve.AddKeyFrame((1.0f / 7.0f) * i, coins[i]);
            }
            clip.AddCurve("Sprite", spriteCurve);
            animator.AddState(new AnimationState("Coin", clip));
            animator.OnUpdate += x =>
            {
                iconContent.Sprite = x.GetSprite("Sprite");
            };
            NewImage("Quad2", default, new vec2(100, 100), Color.Red, horizontalLayout.Transform);
            NewImage("Quad3", default, new vec2(100, 100), Color.Blue, horizontalLayout.Transform);
            NewImage("Quad4", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad5", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad6", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad7", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad8", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            NewImage("Quad9", default, new vec2(100, 100), Color.Green, horizontalLayout.Transform);
            NewImage("Quad10", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
        }

        private static void LogRecursive(Transform current, int depth = 0)
        {
            // Create indentation with dashes based on depth
            string indent = new string('-', depth); // "--", "----", etc.

            Debug.Log($"{indent}{current.Name}");

            // Loop through children and recurse
            for (int i = 0; i < current.Children.Count; i++)
            {
                LogRecursive(current.Children[i], depth + 1);
            }
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
            text.BlockEvents = false;
            text.ReceiveEvents = false;

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