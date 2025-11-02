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
            if (!element || !element.IsEnabled || !element.Actor.IsActiveSelf)
                return;

            element.RectTransform.Recalculate(parent);

            for (int i = element.Transform.Children.Count - 1; i >= 0; i--)
            {
                var child = element.Transform.Children[i];
                if (child.IsEnabled && child.Actor.IsActiveSelf)
                {
                    RebuildRecursive(child.GetComponent<UIElement>(), element.RectTransform, ref mouseEventHandled);
                }
            }

            var mousePos = ScreenToCanvas(Input.MousePosition);
            bool hasMouse = element.RectTransform.Rect.Contains(mousePos);

            if (hasMouse && element.ReceiveEvents && !mouseEventHandled)
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
            for (int i = Transform.Children.Count - 1; i >= 0; --i) 
            {
                bool blocked = false;
                RebuildRecursive(Transform.Children[i].GetComponent<UIElement>(), RectTransform, ref blocked);
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