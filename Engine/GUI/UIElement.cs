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
     
        public virtual void OnRebuild()
        {
        }

        public virtual bool OnPointer(vec2 position, bool pressed)
        {
            return false;
        }
    }
}
