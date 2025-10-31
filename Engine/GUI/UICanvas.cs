using Engine.Types;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    [RequiredComponent(typeof(RectTransform))]
    public class UICanvas : Component, ILateUpdatableComponent
    {
        public RectTransform RectTransform { get; private set; }

        internal override void OnInitialize()
        {
            base.OnInitialize();
            RectTransform = AddComponent<RectTransform>();
            RectTransform.Size = new vec2(512 * 2, 288 * 2);
            RectTransform.Pivot = default;
            RectTransform.Recalculate(null);
        }

        private void RebuildRecursive(UIElement element, RectTransform parent, ref bool mouseEventHandled)
        {
            if (!element)
                return;

            element.RectTransform.Recalculate(parent);

            foreach (var child in element.Transform.Children)
            {
                RebuildRecursive(child.GetComponent<UIElement>(), element.RectTransform, ref mouseEventHandled);
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
                RebuildRecursive(element.GetComponent<UIElement>(), RectTransform, ref blocked);
            }
        }

        public vec2 ScreenToCanvas(vec2 mousePosScreen)
        {
            float x = (mousePosScreen.x / (float)Window.Width) * (float)RectTransform.Rect.Width;
            float y = (mousePosScreen.y / (float)Window.Height) * (float)RectTransform.Rect.Height;
            return new vec2(x, y);
        }
    }
}