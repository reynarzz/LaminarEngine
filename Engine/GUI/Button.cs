using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public class Button : Component, IUpdatableComponent, IPointerDownEvent, IPointerExitEvent, IPointerUpEvent, IPointerEnterEvent, IPointerDragEvent
    {
        public event Action OnButtonClick;
        public UIGraphicsElement Graphic { get; set; }
        public bool UseSprite { get; set; }
        public Sprite NormalSprite { get; set; }
        public Sprite ClickSprite { get; set; }
        public Sprite EnterSprite { get; set; }
        public Sprite DisableSprite { get; set; }

        public Color ActiveColor { get; set; } = Color.Red;
        public Color DisabledColor { get; set; } = new Color(1, 0, 0, 0.5f);
        public Color EnterColor { get; set; } = Color.Yellow;
        public Color ClickColor { get; set; } = Color.Blue;

        private bool _isDragging = false;
        private bool _isPointerDown = false;
        public bool LerpColors { get; set; } = true;
        public float LerpColorSpeed { get; set; } = 5;
        private Color _targetColor;
        private const float _mouseDeltaThreshold = 0.006f;
        private vec2 _pointerDownPos;
        public bool IsDisabled { get; set; }

        public override void OnEnabled()
        {
            base.OnEnabled();

            if (IsDisabled)
            {
                SetOnDisabledGraphic();
            }
            else
            {
                SetOnActiveGraphic();
            }

            Debug.Log("Button enable");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsDisabled)
            {
                return;
            }
            _pointerDownPos = eventData.Position;
            SetOnClickGraphic();
            _isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsDisabled)
            {
                return;
            }
            // Caching values here to avoid issues in case a exception throws while calling 'OnButtonClick?.Invoke()'
            var isPointerDown = _isPointerDown;
            var isDragging = _isDragging;

            _isPointerDown = false;
            _isDragging = false;

            if (isPointerDown)
            {
                SetOnActiveGraphic();
            }

            if (!isDragging && isPointerDown)
            {
                OnButtonClick?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsDisabled)
            {
                return;
            }
            SetOnEnterGraphic();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsDisabled)
            {
                return;
            }
            SetOnActiveGraphic();
        }

        public void OnPointerDrag(PointerEventData eventData)
        {
            if (IsDisabled)
            {
                return;
            }
            var delta = _pointerDownPos - eventData.Position;
            delta /= eventData.Canvas.RectTransform.Rect.Size.x;

            if (_isPointerDown && delta.Magnitude > _mouseDeltaThreshold)
            {
                if (!_isDragging)
                {
                    Cancel();
                }

                _isDragging = true;
            }
        }

        private void Cancel()
        {
            SetOnActiveGraphic();
        }

        private void SetOnActiveGraphic()
        {
            SetOnGraphics(NormalSprite, ActiveColor);
        }
        private void SetOnClickGraphic()
        {
            SetOnGraphics(ClickSprite ?? NormalSprite, ClickColor);
        }

        private void SetOnEnterGraphic()
        {
            SetOnGraphics(EnterSprite ?? NormalSprite, EnterColor);
        }

        private void SetOnDisabledGraphic()
        {
            SetOnGraphics(DisableSprite ?? NormalSprite, DisabledColor);
        }

        private void SetOnGraphics(Sprite sprite, Color color)
        {
            if (Graphic)
            {
                if (UseSprite && sprite)
                {
                    Graphic.Sprite = sprite;
                }
                else
                {
                    _targetColor = color;

                    if (!LerpColors)
                    {
                        Graphic.Color = color;
                    }
                }
            }
        }

        public void OnUpdate()
        {
            if (!UseSprite && Graphic)
            {
                if (LerpColors)
                {
                    Graphic.Color = Color.Lerp(Graphic.Color, _targetColor, LerpColorSpeed * Time.UnscaledDeltaTime);
                }
                else
                {
                    Graphic.Color = _targetColor;
                }
            }
        }
    }
}