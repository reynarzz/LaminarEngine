using Engine.GUI;
using Engine.Types;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [RequiredComponent(typeof(RectTransform))]
    public abstract class UIElement : Renderer2D
    {
        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
        public bool UseCustomSorting { get; set; }
        public bool BlockEvents { get; set; } = true;
        public bool ReceiveEvents { get; set; } = true;

        public virtual void OnPointerDown(vec2 position) { }
        public virtual void OnPointerUp(vec2 position) { }
        public virtual void OnHover(vec2 position) { }

        internal override void Draw()
        {
            Debug.DrawBox(RectTransform.Rect.Center, RectTransform.Rect.Size, Color.Red);
        }
    }
}
