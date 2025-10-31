using Engine.GUI;
using Engine.Types;
using GlmNet;

namespace Engine
{
    [UniqueComponent]
    public class RectTransform : Component
    {
        public vec2 Pivot = new vec2(0.5f, 0.5f);  
        public vec2 Size = new vec2(100f, 100f);   

        public Rect Rect { get; private set; }

        public void Recalculate(RectTransform parent)
        {
            var parentPos = parent?.Rect.Min ?? default;
            var parentSize = parent?.Rect.Size ?? default;

            var parentPivotWorld = parent ? parentPos + new vec2(parent.Pivot.x * parentSize.x,
                                                         parent.Pivot.y * parentSize.y): parentPos;

            var localPos = new vec2(Transform.LocalPosition.x, Transform.LocalPosition.y);
            var childPivotOffset = new vec2(Pivot.x * Size.x, Pivot.y * Size.y);
            var topLeft = parentPivotWorld + localPos - childPivotOffset;

            Rect = new Rect(topLeft, Size);
        }
    }

}
