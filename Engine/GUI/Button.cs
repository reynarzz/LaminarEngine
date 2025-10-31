using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public class Button : Component, IPointerDownEvent, IPointerUpEvent, IPointerHoverEvent
    {
        public event Action OnButtonDown;
        public UIImage Graphic { get; set; }
        public bool UseSprite { get; set; }
        public Sprite NormalSprite { get; set; }
        public Sprite DownSprite { get; set; }
        public Sprite HoverSprite { get; set; }

        public void OnPointerDown(vec2 mousePos)
        {
            OnButtonDown?.Invoke();
            if (UseSprite && Graphic && DownSprite)
            {
                Graphic.Sprite = DownSprite;
            }
        }

        public void OnPointerUp(vec2 mousePos)
        {
            if (UseSprite && Graphic && NormalSprite)
            {
                Graphic.Sprite = NormalSprite;
            }
        }

        public void OnPointerHover(vec2 mousePos)
        {
            if (UseSprite && Graphic && HoverSprite)
            {
                Graphic.Sprite = HoverSprite;
            }
        }
    }
}
