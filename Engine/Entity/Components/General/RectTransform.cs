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

        public void Recalculate(Rect parent)
        {
            var localPos = new vec2(Transform.LocalPosition.x, Transform.LocalPosition.y);
            var pivotOffset = new vec2(Pivot.x * Size.x, Pivot.y * Size.y);
            var topLeft = parent.Min + localPos - new vec2(pivotOffset.x, -pivotOffset.y);

            Rect = new Rect(topLeft, Size);
        }
    }
}
