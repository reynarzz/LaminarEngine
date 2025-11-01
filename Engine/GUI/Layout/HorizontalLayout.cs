using Engine.Types;
using GlmNet;
using System.Collections.Generic;

namespace Engine.GUI
{
    [UniqueComponent]
    public class HorizontalLayoutGroup : UIElement, IUpdatableComponent
    {
        public float Spacing = 5f;
        public bool ResizeToFit { get; set; } = false;
        public vec2 ContentsSize { get; set; } = new vec2(50, 50);

        public vec2 StartPivot { get; set; } = new vec2(0, 0.5f);

        private readonly List<RectTransform> _rectTransforms = new();

        void IUpdatableComponent.OnUpdate()
        {
            ApplyLayout();
        }

        private void ApplyLayout()
        {
            _rectTransforms.Clear();
            SceneManager.ActiveScene.FindAll(_rectTransforms, false, Actor);

            var parentRT = RectTransform;
            float totalWidth = 0f;

            foreach (var child in _rectTransforms)
            {
                if (child.Actor == Actor)
                    continue;

                totalWidth += ContentsSize.x + Spacing;
            }
            if (totalWidth > 0)
            {
                totalWidth -= Spacing;
            }

            float currentX = -totalWidth * StartPivot.x;

            foreach (var child in _rectTransforms)
            {
                if (child.Actor == Actor)
                    continue;

                child.Size = ContentsSize;

                var anchoredPos = new vec2(currentX + child.Pivot.x * child.Size.x, 0);
                child.Transform.LocalPosition = anchoredPos;
                child.Recalculate(parentRT);

                currentX += child.Size.x + Spacing;
            }

            if (ResizeToFit)
            {
                parentRT.Size = new vec2(totalWidth, parentRT.Size.y);
                parentRT.Recalculate(parentRT);
            }
        }
    }
}
