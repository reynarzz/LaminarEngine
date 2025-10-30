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
    public class UIElement : Renderer2D
    {
        public RectTransform RectTransform { get; private set; }
        internal override void OnInitialize()
        {
            base.OnInitialize();
            RectTransform = GetComponent<RectTransform>();
        }

        public virtual void OnRebuild()
        {
        }

        public virtual void Draw(UICanvas canvas, UIDrawList drawList)
        {
        }

        public virtual bool OnPointer(vec2 position, bool pressed)
        {
            return false;
        }
    }
}
