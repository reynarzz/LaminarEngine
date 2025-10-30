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
    [UniqueComponent]
    public class RectTransform : Transform
    {
        public vec2 AnchorMin { get; set; } = new vec2(0.5f, 0.5f);
        public vec2 AnchorMax { get; set; } = new vec2(0.5f, 0.5f);
        public vec2 Pivot { get; set; } = new vec2(0.5f, 0.5f);
        public vec2 OffsetMin { get; set; } = new vec2(0f);
        public vec2 OffsetMax { get; set; } = new vec2(0f);
        public Rect ComputedRect { get; private set; }
        public RectTransform Parent => Transform.Parent as RectTransform;
        public UICanvas Canvas { get; set; }

        public void Recalculate()
        {
            vec2 parentPos;
            vec2 parentSize;

            if (Parent != null)
            {
                parentPos = Parent.ComputedRect.Min;
                parentSize = Parent.ComputedRect.Size;
            }
            else
            {
                parentPos = new vec2(0, 0);
                parentSize = new vec2(Canvas.Width, Canvas.Height);
            }

            vec2 anchorPosMin = parentPos + AnchorMin * parentSize;
            vec2 anchorPosMax = parentPos + AnchorMax * parentSize;
            vec2 size = anchorPosMax - anchorPosMin + (OffsetMax - OffsetMin);
            vec2 pos = anchorPosMin + OffsetMin;
            pos -= size * Pivot;
            ComputedRect = new Rect(pos, size);
        }
    }
}
