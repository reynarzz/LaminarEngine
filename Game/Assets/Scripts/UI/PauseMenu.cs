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
    public class PauseMenu : GameUI
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

        private static Material _backgroundMat;
        private static int _matCount = 0;
        protected override void OnAwake()
        {
            _canvas = new Actor<AudioSource>("Pause menu Canvas").AddComponent<UICanvas>();
            _audioSource = _canvas.GetComponent<AudioSource>();
            _buttonAudioClip = Assets.GetAudioClip("Audio/HALFTONE/UI/1. Buttons/Button_20_adjust.wav");
            _pauseAudioClip = Assets.GetAudioClip("Audio/HALFTONE/UI/1. Buttons/Button_24.wav");
            _canvas.Transform.Parent = Transform;

            var backSize = new vec2(512 * 3, 288 * 3);
            // Background
            _background = UiUtils.NewImage("Background", backSize * 0.5f, backSize, Color32.RGB(200), _canvas.Transform);
            
            _background.BlockEvents = true;
            _background.ReceiveEvents = true;

            if (!_backgroundMat)
            {
                _backgroundMat = new Material(new Shader(Assets.GetText("Shaders/VertScreenGrab.vert").Text, Assets.GetText("Shaders/GrayScale.frag").Text));
            }

            _background.Material = _backgroundMat;
            _background.Material.GetPass(0).IsScreenGrabPass = true;

            // Title text
            _titleText = UiUtils.NewText("Title text", "Pause", new vec2(0, -110), _background.Transform);
            _titleText.FontSize = 70;
            _titleText.Fit = TextFit.ExpandToFit;
            _titleText.Vertical = TextVerticalAlignment.Center;
            _titleText.Transform.WorldScale = vec3.One * 2;
            // TODO: The renderer has a problem with this line.
            _titleText.Material = MaterialUtils.Instance.FontMaterial;
            //-- _titleText.Material = MaterialUtils.FontMaterial; // Use this instead

            var resume = MenuButton("Resume", new vec2(0, 70));
            resume.button.OnButtonClick += OnResume;

            var quit = MenuButton("Quit", new vec2(0, 130));
            quit.button.OnButtonClick += OnQuitGame;

            _graphics.Add(_titleText);
            _graphics.Add(_background);

            // LogRecursive(_background.Transform);

            SetColorAlpha(0, true);
        }

        private void OnQuitGame()
        {
            //--Window.CloseWindow();
        }

        private (UIText text, UIImage image, Button button) MenuButton(string label, vec2 position)
        {
            var buttonSlice = GameTextures.GetAtlas("ui_buttons_long");

            var buttonImage = UiUtils.NewImage(label + "_button image", position, new vec2(150, 40), Color.White, _background.Transform);

            buttonImage.PreserveAspect = true;
            var button = buttonImage.AddComponent<Button>();
            button.Graphic = buttonImage;
            button.UseSprite = true;
            button.NormalSprite = buttonSlice[0];
            button.PressedSprite = buttonSlice[1];
            buttonImage.SortOrder = 2;
            // button.IsDisabled = true;
            // resumeButtonImage.AddComponent<ContentSizeFitter>().Padding = new Thickness(10);

            // resumeButtonImage.AddComponent<Test_UIEvent>();

            var text = UiUtils.NewText(label + "_text", label, default, buttonImage.Transform);
            text.ReceiveEvents = false;
            text.RectTransform.Size.y = buttonImage.RectTransform.Size.y;
            text.RectTransform.Size.x = buttonImage.RectTransform.Size.x;
            text.FontSize = 30;
            text.Padding.Bottom = 5;
            text.SortOrder = 2;
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
            _graphics.Add(buttonImage);

            return (text, buttonImage, button);
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
#if RELEASE

            WindowManager.Window.CursorVisible = false;
#endif
            _show = false;
        }

        public void OnPause()
        {
            //Actor.IsActiveSelf = !Actor.IsActiveSelf;
            _show = !_show;
            Time.TimeScale = _show ? 0 : 1;
#if RELEASE
            WindowManager.Window.CursorVisible = _show;
#endif
            if (_show)
            {
                _audioSource.PlayOneShot(_pauseAudioClip, 0.5f);
            }
            else
            {
                _audioSource.PlayOneShot(_buttonAudioClip, 0.5f);
            }

        }
        void SetColorAlpha(float alpha, bool immediate = false)
        {
            foreach (var item in _graphics)
            {
                var color = item.Color;
                color.A = alpha;
                if (!immediate)
                {
                    item.Color = Color.MoveTowards(item.Color, color, Time.UnscaledDeltaTime * 5f);
                }
                else
                {
                    item.Color = color;
                }
            }
        }

        protected override void OnLateUpdate()
        {


            if (_show)
            {
                SetColorAlpha(1);
            }
            else
            {
                SetColorAlpha(0);
            }

        }
    }
}