using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public class UICanvas : Component, ILateUpdatableComponent
    {
        public float Width { get; internal set; } = 512 * 2;
        public float Height { get; internal set; } = 288 * 2;

        private void RebuildRecursive(UIElement element, Rect parentRect, ref bool mouseEventHandled)
        {
            if (!element)
                return;

            element.RectTransform.Recalculate(parentRect);

            foreach (var child in element.Transform.Children)
            {
                RebuildRecursive(child.GetComponent<UIElement>(), element.RectTransform.Rect, ref mouseEventHandled);
            }

            if (mouseEventHandled)
            {
                if (element is UIGraphicsElement graphic)
                {
                    graphic.OnCanvasDraw(this);
                }
                return;
            }

            var mousePos = ScreenToCanvas(Input.MousePosition);
            bool hasMouse = element.RectTransform.Rect.Contains(mousePos);

            if (hasMouse && element.ReceiveEvents)
            {
                if (Input.GetMouseDown(MouseButton.Left))
                {
                    element.GetComponent<IPointerDownEvent>()?.OnPointerDown(mousePos);
                }

                if (Input.GetMouseUp(MouseButton.Left))
                {
                    element.GetComponent<IPointerUpEvent>()?.OnPointerUp(mousePos);
                }

                element.GetComponent<IPointerHoverEvent>()?.OnPointerHover(mousePos);

                if (element.BlockEvents)
                {
                    mouseEventHandled = true;
                }
            }

            if (element is UIGraphicsElement graphics)
            {
                graphics.OnCanvasDraw(this);
            }
        }

        public void OnLateUpdate()
        {
            foreach (var element in Transform.Children)
            {
                bool blocked = false;
                RebuildRecursive(element.GetComponent<UIElement>(), new Rect(0, 0, Width, Height), ref blocked);
            }
        }

        public vec2 ScreenToCanvas(vec2 mousePosScreen)
        {
            float x = (mousePosScreen.x / (float)Window.Width) * (float)Width;
            float y = (mousePosScreen.y / (float)Window.Height) * (float)Height;
            return new vec2(x, y);
        }
    }
}