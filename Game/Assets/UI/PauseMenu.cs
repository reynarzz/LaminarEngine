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

        public override void OnStart()
        {
            _canvas = new Actor("Pause menu Canvas").AddComponent<UICanvas>();
            _canvas.Transform.Parent = Transform;

            // Background
            _background = NewImage("Background", new vec2(512, 0), new vec2(512 * 2, int.MaxValue), Color.Black * 0.5f, _canvas.Transform);
            _background.BlockEvents = false;
            _background.ReceiveEvents = false;
            // Title text
            _titleText = NewText("Title text", "Pause", new vec2(512, 160), _canvas.Transform);
            _titleText.FontSize = 50;
            var resumeButtonImage = NewImage("Resume button image", new vec2(0, 400), new vec2(200, 40), Color.Gray, _background.Transform);

            resumeButtonImage.AddComponent<Button>().OnButtonDown += OnResume;
            var text = NewText("Resume text", "Resume", default, resumeButtonImage.Transform);
            text.ReceiveEvents = false;
            text.RectTransform.Size.y = resumeButtonImage.RectTransform.Size.y;
            Actor.IsActiveSelf = false;
            //Time.TimeScale = 0;
        }

        private void OnResume()
        {
            Debug.Log("Button down: ");
            Time.TimeScale = 1;

            Actor.IsActiveSelf = false;
        }

        public void OnPause()
        {
            Actor.IsActiveSelf = !Actor.IsActiveSelf;
            Time.TimeScale = Actor.IsActiveSelf ? 0 : 1;
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