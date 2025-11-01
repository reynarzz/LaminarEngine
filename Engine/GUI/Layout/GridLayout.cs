using Engine.Types;
using GlmNet;
using System.Collections.Generic;

namespace Engine.GUI
{
    [UniqueComponent]
    public class GridLayout : UIElement, IUpdatableComponent
    {
        public float Spacing { get; set; } = 5f;
        public int MaxPerRow { get; set; } = 3;

        public bool ResizeToFitHorizontal { get; set; } = false;
        public bool ResizeToFitVertical { get; set; } = false;

        public vec2 ContentsSize { get; set; } = new vec2(50, 50);
        public vec2 StartPivot { get; set; } = new vec2(0, 1); 
        public Thickness Padding = new Thickness(0);

        private readonly List<RectTransform> _rectTransforms = new();

        void IUpdatableComponent.OnUpdate() => ApplyGrid();

        private void ApplyGrid()
        {
            if (MaxPerRow < 1) MaxPerRow = 1;

            _rectTransforms.Clear();
            SceneManager.ActiveScene.FindAll(_rectTransforms, false, Actor);
            var parentRT = RectTransform;

            int count = 0;
            foreach (var rt in _rectTransforms)
                if (rt.Actor != Actor) count++;

            int rows = (count + MaxPerRow - 1) / MaxPerRow;

            float gridWidth = MaxPerRow * ContentsSize.x + (MaxPerRow - 1) * Spacing;
            float gridHeight = rows * ContentsSize.y + (rows - 1) * Spacing;

            // Include padding
            float totalWidth = gridWidth + Padding.Left + Padding.Right;
            float totalHeight = gridHeight + Padding.Top + Padding.Bottom;

            // Origins
            float startX = -totalWidth * StartPivot.x + Padding.Left;
            float startY = -totalHeight * StartPivot.y + Padding.Top; 

            float currentX = startX;
            float currentY = startY;

            int col = 0;
            foreach (var child in _rectTransforms)
            {
                if (child.Actor == Actor) continue;

                child.Size = ContentsSize;

                float posX = currentX + child.Pivot.x * child.Size.x;
                float posY = currentY + child.Pivot.y * child.Size.y; 

                child.Transform.LocalPosition = new vec2(posX, posY);
                child.Recalculate(parentRT);

                col++;
                currentX += ContentsSize.x + Spacing;

                if (col >= MaxPerRow)
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
