using Engine.Types;
using GlmNet;
using System.Collections.Generic;

namespace Engine.GUI
{
    [UniqueComponent]
    public class HorizontalLayout : UIElement, IUpdatableComponent
    {
        public float Spacing = 5f;
        public bool ResizeToFitHorizontal { get; set; } = false;
        public bool ResizeToFitVertical { get; set; } = false;

        public vec2 ContentsSize { get; set; } = new vec2(50, 50);
        public vec2 StartPivot { get; set; } = new vec2(0, 0.5f);
        public Thickness Padding = new Thickness(0);

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
                totalWidth -= Spacing;

            float totalWidthWithPadding = totalWidth + Padding.Left + Padding.Right;

            float currentX = -totalWidthWithPadding * StartPivot.x + Padding.Left;
            float maxChildHeight = 0f; 

            foreach (var child in _rectTransforms)
            {
                if (child.Actor == Actor)
                    continue;

                child.Size = ContentsSize;

                float yOffset = 0f;

                child.Transform.LocalPosition = new vec2(currentX + child.Pivot.x * child.Size.x, yOffset);
                child.Recalculate(parentRT);

                maxChildHeight = MathF.Max(maxChildHeight, child.Rect.Size.y);

                currentX += child.Size.x + Spacing;
            }

            if (ResizeToFitHorizontal)
            {
                parentRT.Size = new vec2(totalWidthWithPadding, parentRT.Size.y);
                parentRT.Recalculate(parentRT);
            }

            if (ResizeToFitVertical)
            {
                float newHeight = maxChildHeight + Padding.Top + Padding.Bottom;
                parentRT.Size = new vec2(parentRT.Size.x, newHeight);
                parentRT.Recalculate(parentRT);
            }
        }
    }
}
