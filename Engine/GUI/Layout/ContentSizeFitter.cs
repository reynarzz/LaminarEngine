using Engine.Types;

namespace Engine.GUI
{
    [UniqueComponent]
    public class ContentSizeFitter : UIElement
    {
        public bool FitHorizontal { get; set; } = true;
        public bool FitVertical { get; set; } = true;
        public Thickness Padding { get; set; } = new Thickness(0);

        private readonly List<RectTransform> _children = new();

        public void ResizeToFitChildren()
        {
            _children.Clear();

            foreach (var child in Transform.Children)
            {
                var rect = child.GetComponent<UIElement>();
                if (rect && child.Actor.IsActiveSelf && rect.IsEnabled)
                {
                    _children.Add(rect.RectTransform);
                }
            }

            if (_children.Count == 0) 
                return;

            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            foreach (var child in _children)
            {
                var rect = child.Rect;

                minX = Math.Min(minX, rect.X);
                minY = Math.Min(minY, rect.Y);
                maxX = Math.Max(maxX, rect.X + rect.Width);
                maxY = Math.Max(maxY, rect.Y + rect.Height);
            }

            float contentWidth = maxX - minX;
            float contentHeight = maxY - minY;

            contentWidth += Padding.Left + Padding.Right;
            contentHeight += Padding.Top + Padding.Bottom;

            var size = RectTransform.Size;
            if (FitHorizontal) 
            {
                size.x = contentWidth; 
            }
            if (FitVertical) 
            { 
                size.y = contentHeight; 
            }
            RectTransform.Size = size;
            RectTransform.Recalculate();
            _children.Clear();
        }
    }
}
