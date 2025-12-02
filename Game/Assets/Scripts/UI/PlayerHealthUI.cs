using Engine;
using Engine.GUI;
using GlmNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class PlayerHealthUI : GameUI
    {
        private UIImage[] _heartsImages;
        private UIImage _healthFrame;
        private const int _minHeartsSlots = 3;
        private const int _maxHeartsSlots = 10;
        private int _enabledHeartsCount;
        private float _hearthAnimTime = 0;
        protected override void OnAwake()
        {
            var heartCanvas = new Actor("Canvas Health").AddComponent<UICanvas>();
            heartCanvas.Transform.Parent = Transform;

            var sprites = GameTextures.GetAtlas("small_heart_idle");

            UIImage Image(string name, vec2 position, vec2 size, Sprite sprite, Transform parent)
            {
                var image = new Actor(name).AddComponent<UIImage>();
                image.Transform.Parent = parent.Transform;
                image.RectTransform.Pivot = new vec2(0.0f, 0.0f);
                image.RectTransform.Size = size;
                image.Material = MaterialUtils.UIMaterial;
                image.Sprite = sprite;
                image.PreserveAspect = true;
                image.Transform.LocalPosition = position;
                return image;
            }

            const float uiSizeMult = 3;
            var lifebarSprites = GameTextures.GetAtlas("health_bar_frame");
            _healthFrame = Image("Life bar", new vec2(10, 10), new vec2(143, 34) * uiSizeMult, lifebarSprites[^1], heartCanvas.Transform);

            _heartsImages = new UIImage[lifebarSprites.Length + 2];
            for (int i = 0; i < _heartsImages.Length; i++)
            {
                _heartsImages[i] = Image("Heart1", new vec2(67 + 32 * i, 50), new vec2(8, 7) * uiSizeMult, sprites[0], _healthFrame.Transform);
                _heartsImages[i].RectTransform.Pivot = vec2.Half;
            }

            SetMaxHeartsSlots(10);
            SetMaxHeartsSlots(5);
            StartCoroutine(HeartsAnim());
        }

        public void InitHealth(int count)
        {
            SetMaxHeartsSlots(count);
            UpdatePlayerHealth(count);
        }

        public void SetMaxHeartsSlots(int count)
        {
            count = Math.Clamp(count, _minHeartsSlots, _maxHeartsSlots);
            _healthFrame.Sprite = GameTextures.GetAtlas("health_bar_frame")[count - 3];
        }

        public void UpdatePlayerHealth(int count)
        {
            count = Math.Clamp(count, 0, _maxHeartsSlots);
            _enabledHeartsCount = count;
            for (int i = 0; i < _heartsImages.Length; i++)
            {
                var heartImage = _heartsImages[i];
                heartImage.IsEnabled = i < count;

                if (i < count - 1)
                {
                    heartImage.Transform.LocalScale = vec3.One;
                }
            }

            _hearthAnimTime = 0;
        }

        private IEnumerator HeartsAnim()
        {
            while (true)
            {
                if (_enabledHeartsCount > 0)
                {
                    var heartImage = _heartsImages[_enabledHeartsCount - 1];
                    heartImage.Transform.LocalScale = vec3.One + ((vec3.Half * MathF.Sin(_hearthAnimTime * 5)) + vec3.Half) * 0.2f;
                    _hearthAnimTime += Time.DeltaTime;
                }
                yield return 0;
            }
        }

    }
}
