using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public class Button : Component, IPointerDownEvent, IPointerExitEvent, IPointerUpEvent, IPointerEnterEvent, IPointerDragEvent
    {
        public event Action OnButtonClick;
        public UIImage Graphic { get; set; }
        public bool UseSprite { get; set; }
        public Sprite NormalSprite { get; set; }
        public Sprite ClickSprite { get; set; }
        public Sprite PointerEnterSprite { get; set; }
        private bool _isDragging = false;
        private bool _isPointerDown = false;

        private const float _mouseDeltaThreshold = 0.7f;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (UseSprite && Graphic && ClickSprite)
            {
                Graphic.Sprite = ClickSprite;
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
            UsePointerEnterSprite();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isDragging)
            {
                UseNormalSprite();
            }
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

        private void UsePointerEnterSprite()
        {
            if (UseSprite && Graphic && PointerEnterSprite)
            {
                Graphic.Sprite = PointerEnterSprite;
            }
        }

    }
}