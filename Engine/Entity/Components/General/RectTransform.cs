using Engine.GUI;
using Engine.Types;
using GlmNet;

namespace Engine
{
    [UniqueComponent]
    public class RectTransform : Component
    {
        public vec2 Pivot = new vec2(0.5f, 0.5f);
        public float Width = 100f;
        public float Height = 100f;

        public Rect ComputedRect { get; private set; }

        public void Recalculate(UICanvas canvas)
        {
            vec2 size = new vec2(Width, Height);
            vec2 topLeft = new vec2(Transform.LocalPosition) - new vec2(Pivot.x * size.x, -Pivot.y * size.y);
            ComputedRect = new Rect(topLeft, size);
        }
    }
}
