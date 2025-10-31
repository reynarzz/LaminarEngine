using Engine.GUI;
using Engine.Types;
using GlmNet;
using System;

namespace Engine
{
    [UniqueComponent]
    public class RectTransform : Transform
    {
        public vec2 AnchorMin { get; set; } = new vec2(0.5f, 0.5f);
        public vec2 AnchorMax { get; set; } = new vec2(0.5f, 0.5f);
        public vec2 Pivot { get; set; } = new vec2(0.5f, 0.5f);
        public vec2 SizeDelta { get; set; } = new vec2(100, 100);
        public vec2 AnchoredPosition { get; set; } = new vec2(0, 0);
        public Rect ComputedRect { get; private set; }

        public RectTransform Parent => Transform.Parent as RectTransform;

        public void Recalculate(UICanvas canvas)
        {
            vec2 parentPos, parentSize;
            if (Parent != null)
            {
                parentPos = Parent.ComputedRect.Min;
                parentSize = Parent.ComputedRect.Size;
            }
            else
            {
                parentPos = new vec2(0, 0);
                parentSize = new vec2(canvas.Width, canvas.Height);
            }

            var anchorPosMin = parentPos + AnchorMin * parentSize;
            var anchorPosMax = parentPos + AnchorMax * parentSize;
            var anchorSize = anchorPosMax - anchorPosMin;

            vec2 size;
            if (AnchorMin == AnchorMax)
            {
                // Fixed-size UI element
                size = SizeDelta;
            }
            else
            {
                size = anchorSize + SizeDelta;

                // if fully stretched, ignore SizeDelta entirely
                if (AnchorMin == new vec2(0, 0) && AnchorMax == new vec2(1, 1))
                {
                    size = anchorSize;
                }
            }
            vec2 anchorCenter = anchorPosMin + 0.5f * anchorSize; 
            vec2 pivotPos = anchorCenter + AnchoredPosition;
            vec2 minCorner = pivotPos - size * Pivot;

            ComputedRect = new Rect(minCorner, size);

            Transform.LocalPosition = new vec3(pivotPos, Transform.LocalPosition.z);
        }

        public vec2 OffsetMin
        {
            get
            {
                if (Parent == null) return new vec2(0);
                vec2 parentSize = Parent.ComputedRect.Size;
                vec2 anchorPosMin = AnchorMin * parentSize;
                return ComputedRect.Min - (Parent.ComputedRect.Min + anchorPosMin);
            }
            set
            {
                if (Parent == null) return;
                vec2 parentSize = Parent.ComputedRect.Size;
                vec2 anchorPosMin = AnchorMin * parentSize;
                vec2 anchorPosMax = AnchorMax * parentSize;

                vec2 anchorSize = anchorPosMax - anchorPosMin;
                vec2 offsetMax = OffsetMax;

                SizeDelta = anchorSize - (offsetMax - value);
                AnchoredPosition = new vec2(
                    value.x + SizeDelta.x * Pivot.x,
                    value.y + SizeDelta.y * Pivot.y
                );
            }
        }

        public vec2 OffsetMax
        {
            get
            {
                if (Parent == null) return new vec2(0);
                vec2 parentSize = Parent.ComputedRect.Size;
                vec2 anchorPosMax = AnchorMax * parentSize;
                return (Parent.ComputedRect.Min + anchorPosMax) - ComputedRect.Max;
            }
            set
            {
                if (Parent == null) return;
                vec2 parentSize = Parent.ComputedRect.Size;
                vec2 anchorPosMin = AnchorMin * parentSize;
                vec2 anchorPosMax = AnchorMax * parentSize;

                vec2 anchorSize = anchorPosMax - anchorPosMin;
                vec2 offsetMin = OffsetMin;

                SizeDelta = anchorSize - (value - offsetMin);
                AnchoredPosition = new vec2(
                    offsetMin.x + SizeDelta.x * Pivot.x,
                    offsetMin.y + SizeDelta.y * Pivot.y
                );
            }
        }
    }
}
