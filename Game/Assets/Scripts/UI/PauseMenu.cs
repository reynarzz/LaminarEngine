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
    // NOTE: This is a test class
    public class PauseMenu : ScriptBehavior
    {
        private UIImage _background;
        private UIImage _buttonImage;
        private UIText _buttonText;
        private Button _button;

        private UICanvas _canvas;
        private UIText _titleText;
        private bool _show = false;
        private UIElement _inventory;
        private AudioSource _audioSource;
        private AudioClip _buttonAudioClip;
        private AudioClip _pauseAudioClip;
        private List<UIGraphicsElement> _graphics = new();
        public override void OnAwake()
        {
            _canvas = new Actor<AudioSource>("Pause menu Canvas").AddComponent<UICanvas>();
            _audioSource = _canvas.GetComponent<AudioSource>();
            _buttonAudioClip = Assets.GetAudioClip("Audio/HALFTONE/UI/1. Buttons/Button_20_adjust.wav");
            _pauseAudioClip = Assets.GetAudioClip("Audio/HALFTONE/UI/1. Buttons/Button_24.wav");
            _canvas.Transform.Parent = Transform;

            var backSize = new vec2(512 * 3, 288 * 3);
            // Background
            _background = NewImage("Background", backSize * 0.5f, backSize, Color.White, _canvas.Transform);
            _background.BlockEvents = true;
            _background.ReceiveEvents = true;
            _background.Material = new Material(new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/GrayScale.frag").Text));
            _background.Material.Passes[0].IsScreenGrabPass = true;

            Inventory();
            var fontMat = new Material(new Shader(Assets.GetText("Shaders/Font/FontVert.vert").Text, Assets.GetText("Shaders/Font/FontFrag.frag").Text));

            // Title text
            _titleText = NewText("Title text", "Pause", new vec2(0, -110), _background.Transform);
            _titleText.FontSize = 70;
            _titleText.Fit = TextFit.ExpandToFit;
            _titleText.Vertical =  TextVerticalAlignment.Center;
            _titleText.Material = fontMat;

            var buttonSlice = GameTextureAtlases.GetAtlas("ui_buttons_long");

            var resumeButtonImage = NewImage("Resume button image", new vec2(0, 0), new vec2(150, 40), Color.White, _background.Transform);

            resumeButtonImage.PreserveAspect = true;
            var button = resumeButtonImage.AddComponent<Button>();
            button.OnButtonClick += OnResume;
            button.Graphic = resumeButtonImage;
            button.UseSprite = true;
            button.NormalSprite = buttonSlice[0];
            button.PressedSprite = buttonSlice[1];

            // button.IsDisabled = true;
            // resumeButtonImage.AddComponent<ContentSizeFitter>().Padding = new Thickness(10);

            // resumeButtonImage.AddComponent<Test_UIEvent>();

            var text = NewText("Resume text", "Resume", default, resumeButtonImage.Transform);
            text.ReceiveEvents = false;
            text.RectTransform.Size.y = resumeButtonImage.RectTransform.Size.y;
            text.RectTransform.Size.x = resumeButtonImage.RectTransform.Size.x;
            text.FontSize = 30;
            text.Padding.Bottom = 5;
            button.OnPointerDownEvent += () => text.Padding.Bottom -= 10;
            button.OnPointerUpEvent += () => text.Padding.Bottom += 10;
            button.OnPointerCancelEvent += () => text.Padding.Bottom += 10;
            // text.Wrap = TextWrap.WordWrap;

            text.Horizontal = TextHorizontalAlignment.Center;
            text.Vertical = TextVerticalAlignment.Center;
            //text.Padding.Right = 10;
            //text.Padding.Left = 10;
            //Time.TimeScale = 0;
            //text.AddComponent<Test_UIEvent>();

            _graphics.Add(text);
            _graphics.Add(_titleText);
            _graphics.Add(_background);
            _graphics.Add(resumeButtonImage);

           // LogRecursive(_background.Transform);
        }

        private void Inventory()
        {
            _inventory = new Actor<UIElement, ContentSizeFitter>().GetComponent<UIElement>();
            _inventory.Transform.Parent = _background.Transform;
            _inventory.Transform.LocalPosition = new vec3(0, 298);

            var inventory = NewImage("Inventory image", default, new vec2(320, 240), Color.White, _inventory.Transform);
            inventory.AddComponent<ContentSizeFitter>();
            inventory.RectTransform.Pivot = new vec2(0.5f, 0.5f);
            var img = inventory.GetComponent<UIImage>();
            img.Material = GameManager.DefaultMaterial;
            img.Sprite = new Sprite(Assets.GetTexture("pixel-ui_panel.png"));
            img.IsSliced = true;
            img.SlicedBorderResolution = 2.5f;

            var inventoryTitleText = NewText("inventory title", "Inventory", new vec2(0, -52), _inventory.Transform);
            inventoryTitleText.Fit = TextFit.ExpandToFit;
            inventoryTitleText.FontSize = 30;
            inventoryTitleText.OutlineSize = 0;
            inventoryTitleText.FontResolution = 10;
            //inventoryTitleText.Transform.LocalScale = vec3.One * 0.3f;

            var horizontalLayout = new Actor("HorizontalRect").AddComponent<GridLayout>();
            horizontalLayout.Transform.Parent = inventory.Transform;
            horizontalLayout.Transform.LocalPosition = new vec3(0, 0);
            horizontalLayout.ResizeToFitVertical = true;
            horizontalLayout.ResizeToFitHorizontal = true;
            horizontalLayout.Spacing = 4;
            horizontalLayout.Padding = new Thickness(12);
            horizontalLayout.Padding.Top = 38;
            horizontalLayout.StartPivot = new vec2(0.5f, 0.5f);
            horizontalLayout.MaxPerRow = 10;
            horizontalLayout.ContentsSize = new vec2(46,46);
            var slotSprite = new Sprite(Assets.GetTexture("pixel-ui_slot.png"));

            var parent = NewImage("Quad1", default, new vec2(100, 100), Color.White, horizontalLayout.Transform);
            parent.Sprite = slotSprite;
            var coins = GameTextureAtlases.GetAtlas("coin_currency");


            var iconContent = NewImage("Content", default, new vec2(34, 34), Color.White, parent.Transform);
            iconContent.RectTransform.Pivot = new vec2(0.5f, 0.6f);
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

            for (int i = 0; i < 19; i++)
            {
                NewImage("Quad2", default, new vec2(100, 100), Color.White, horizontalLayout.Transform).Sprite = slotSprite;

            }

            _inventory.Actor.IsActiveSelf = false;
        }

        private static void LogHierarchy(Transform current, int depth = 0)
        {
            string indent = new string('-', depth);

            Debug.Log($"{indent}{current.Name}");

            for (int i = 0; i < current.Children.Count; i++)
            {
                LogHierarchy(current.Children[i], depth + 1);
            }
        }
        private void OnResume()
        {
            Time.TimeScale = 1;
            //Actor.IsActiveSelf = false;

            if (_show)
            {
                _audioSource.PlayOneShot(_buttonAudioClip, 0.5f);
            }
            _show = false;
        }

        public void OnPause()
        {
            //Actor.IsActiveSelf = !Actor.IsActiveSelf;
            _show = !_show;
            Time.TimeScale = _show ? 0 : 1;

            if(_show)
            _audioSource.PlayOneShot(_pauseAudioClip, 0.5f);
            else
            {
                _audioSource.PlayOneShot(_buttonAudioClip, 0.5f);
            }

        }

        public override void OnLateUpdate()
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

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _inventory.Actor.IsActiveSelf = !_inventory.Actor.IsActiveSelf;
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