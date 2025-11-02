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

        public void OnPointerDown(PointerEventData eventData)
        {
            if (UseSprite && Graphic && DownSprite)
            {
                Graphic.Sprite = DownSprite;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (UseSprite && Graphic && NormalSprite)
            {
                Graphic.Sprite = NormalSprite;
            }

            if (!_isDragging)
            {
                OnButtonClick?.Invoke();
            }

            _isDragging = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (UseSprite && Graphic && HoverSprite)
            {
                Graphic.Sprite = HoverSprite;
            }
        }

        public void OnPointerDrag(PointerEventData eventData)
        {
            _isDragging = true;

            // TODO: perform cancel.
        }
    }
}