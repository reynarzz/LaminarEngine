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
    public abstract class UIElement : Renderer2D // TODO: remove renderer2d
    {
        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
        public bool UseCustomSorting { get; set; }
        public bool BlockEvents { get; set; } = true;
        public bool ReceiveEvents { get; set; } = true;

        internal sealed override void Draw()
        {
#if DEBUG
            if (Debug.DrawUILines)
            {
                Debug.DrawBoxUI(RectTransform.Rect.Center, RectTransform.Rect.Size, Color.Red);
            }
#endif
        }
    }
}
