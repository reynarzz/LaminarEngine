using Engine.GUI;
using Engine.Layers;
using Engine.Types;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    [RequireComponent(typeof(RectTransform))]
    public class UIElement : Renderer2D // TODO: remove renderer2d
    {
        private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
        public bool UseCustomSorting { get; set; }
        public bool BlockEvents { get; set; } = true;
        public bool ReceiveEvents { get; set; } = true;

        internal override void OnInternalInitialize()
        {
            base.OnInternalInitialize();

            RenderingLayer.PushUIRenderer(this);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();

            RenderingLayer.PushUIRenderer(this);
        }

        internal sealed override void Draw()
        {
#if DEBUG
            if (Debug.DrawUILines)
            {


                vec2 pivotOffset = new vec2(RectTransform.Rect.Width * (0.5f - RectTransform.Pivot.x),
                                            RectTransform.Rect.Height * (0.5f - RectTransform.Pivot.y));

                vec2 centerWS = new vec2(Transform.WorldPosition.x + pivotOffset.x,
                                         Transform.WorldPosition.y + pivotOffset.y);

                Debug.DrawBoxUI(centerWS, RectTransform.Rect.Size, Transform.WorldEulerAngles, Color.Red);
            }
#endif
        }
    }
}
