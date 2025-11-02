using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public class Button : Component, IPointerDownEvent, IPointerUpEvent, IPointerEnterEvent, IDragEvent
    {
        public event Action OnButtonClick;
        public UIImage Graphic { get; set; }
        public bool UseSprite { get; set; }
        public Sprite NormalSprite { get; set; }
        public Sprite DownSprite { get; set; }
        public Sprite HoverSprite { get; set; }
        private bool _isDragging = false;
        private bool _isPointerDown = false;

        private const float _mouseDeltaThreshold = 0.2f;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (UseSprite && Graphic && DownSprite)
            {
                Graphic.Sprite = DownSprite;
            }
            _isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // Caching values here to avoid issues in case a exception throws while calling 'OnButtonClick?.Invoke()'
            var isPointerDown = _isPointerDown;
            var isDragging = _isDragging;

            _isPointerDown = false;
            _isDragging = false;

            if (isPointerDown)
            {
                UseNormalSprite();
            }

            if (!isDragging && isPointerDown)
            {
                OnButtonClick?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UseHoverSprite();
        }

        public void OnPointerDrag(PointerEventData eventData)
        {
            if (_isPointerDown && eventData.Delta.Magnitude > _mouseDeltaThreshold)
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
            UseNormalSprite();
        }

        private void UseNormalSprite()
        {
            if (UseSprite && Graphic && NormalSprite)
            {
                Graphic.Sprite = NormalSprite;
            }
        }

        private void UseHoverSprite()
        {
            if (UseSprite && Graphic && HoverSprite)
            {
                Graphic.Sprite = HoverSprite;
            }
        }
    }
}