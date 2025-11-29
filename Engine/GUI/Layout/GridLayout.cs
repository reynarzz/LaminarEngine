using Engine.Types;
using GlmNet;

namespace Engine.GUI
{
    [UniqueComponent]
    public class GridLayout : UIElement
    {
        public float Spacing { get; set; } = 5f;
        public int MaxPerRow { get; set; } = 3;

        public bool ResizeToFitHorizontal { get; set; } = false;
        public bool ResizeToFitVertical { get; set; } = false;

        public vec2 ContentsSize { get; set; } = new vec2(50, 50);
        public vec2 StartPivot { get; set; } = new vec2(0, 1); 
        public Thickness Padding = new Thickness(0);

        private readonly List<RectTransform> _rectTransforms = new();

        public void RecalculateLayout()
        {
            if (MaxPerRow < 1) MaxPerRow = 1;

            _rectTransforms.Clear();

            var parentRT = RectTransform;

            foreach (var rt in Transform.Children)
            {
                var rectTransform = rt.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    _rectTransforms.Add(rectTransform);
                }
            }

            int count = _rectTransforms.Count;

            if (count == 0)
                return;

            var maxPerRowAdjusted = Math.Min(MaxPerRow, count);
            int rows = (count + maxPerRowAdjusted - 1) / maxPerRowAdjusted;

            float gridWidth = maxPerRowAdjusted * ContentsSize.x + (maxPerRowAdjusted - 1) * Spacing;
            float gridHeight = rows * ContentsSize.y + (rows - 1) * Spacing;

            float totalWidth = gridWidth + Padding.Left + Padding.Right;
            float totalHeight = gridHeight + Padding.Top + Padding.Bottom;

            float startX = -totalWidth * StartPivot.x + Padding.Left;
            float startY = -totalHeight * StartPivot.y + Padding.Top; 

            float currentX = startX;
            float currentY = startY;

            int col = 0;
            foreach (var child in _rectTransforms)
            {
                if (child.Actor == Actor || child.Transform.Parent != Transform) continue;
                
                child.Size = ContentsSize;

                float posX = currentX + child.Pivot.x * child.Size.x;
                float posY = currentY + child.Pivot.y * child.Size.y; 

                child.Transform.LocalPosition = new vec2(posX, posY);
                child.Recalculate(parentRT);

                col++;
                currentX += ContentsSize.x + Spacing;

                if (col >= maxPerRowAdjusted)
                {
                    col = 0;
                    currentX = startX;
                    currentY += ContentsSize.y + Spacing; 
                }
            }

            if (ResizeToFitHorizontal)
            {
                parentRT.Size = new vec2(totalWidth, parentRT.Size.y);
                parentRT.Recalculate(parentRT);
            }

            if (ResizeToFitVertical)
            {
                parentRT.Size = new vec2(parentRT.Size.x, totalHeight);
                parentRT.Recalculate(parentRT);
            }
        }
    }
}
