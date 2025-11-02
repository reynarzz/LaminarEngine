using Engine.Types;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public struct PointerEventData
    {
        public vec2 Position { get; set; }
        public vec2 Delta { get; set; }
    }

    [RequiredComponent(typeof(RectTransform))]
    public class UICanvas : Component, ILateUpdatableComponent, IUpdatableComponent
    {
        public RectTransform RectTransform { get; private set; }
        private vec2 _prevMousePos;
        private vec2 _mouseCanvasPos;
        private vec2 _mousePosDelta;
        private List<IPointerEnterEvent> _pointerEnterEvents = new();
        private List<IPointerExitEvent> _pointerExitEvents = new();
        private List<IPointerDownEvent> _pointerDownEvents = new();
        private List<IPointerUpEvent> _pointerUpEvents = new();
        private List<IDragEvent> _pointerDragEvents = new();

        private UIElement _dragElement;
        private UIElement _pointerEnterElement;

        internal override void OnInitialize()
        {
            base.OnInitialize();
            RectTransform = AddComponent<RectTransform>();
            RectTransform.Size = new vec2(512 * 2, 288 * 2);
            RectTransform.Pivot = default;
            RectTransform.Recalculate(null);
        }
        private void EventRecursive(UIElement element, RectTransform parent, ref bool mouseEventHandled)
        {
            if (!element || !element.IsEnabled || !element.Actor.IsActiveSelf)
                return;

            for (int i = element.Transform.Children.Count - 1; i >= 0; i--)
            {
                var child = element.Transform.Children[i];
                if (child.IsEnabled && child.Actor.IsActiveSelf)
                {
                    EventRecursive(child.GetComponent<UIElement>(), element.RectTransform, ref mouseEventHandled);
                }
            }

            bool hasMouse = element.RectTransform.Rect.Contains(_mouseCanvasPos);
            var eventData = new PointerEventData();
            eventData.Position = _mouseCanvasPos;
            eventData.Delta = _mousePosDelta;

            if (hasMouse && element.ReceiveEvents && !mouseEventHandled)
            {
                if (Input.GetMouseDown(MouseButton.Left))
                {
                    _pointerDownEvents.Clear();
                    element.GetComponents(ref _pointerDownEvents);
                    foreach (var evt in _pointerDownEvents)
                    {
                        if ((evt as Component).IsEnabled)
                        {
                            evt.OnPointerDown(eventData);
                        }
                    }

                    _dragElement = element;
                }

                if (Input.GetMouseUp(MouseButton.Left))
                {
                    _pointerUpEvents.Clear();
                    element.GetComponents(ref _pointerUpEvents);
                    _dragElement = null;
                    foreach (var evt in _pointerUpEvents)
                    {
                        if ((evt as Component).IsEnabled)
                        {
                            evt.OnPointerUp(eventData);
                        }
                    }
                }

                if (_pointerEnterElement != element)
                {
                    _pointerEnterElement = element;
                    _pointerEnterEvents.Clear();
                    element.GetComponents(ref _pointerEnterEvents);

                    foreach (var evt in _pointerEnterEvents)
                    {
                        if ((evt as Component).IsEnabled)
                        {
                            evt.OnPointerEnter(eventData);
                        }
                    }
                }

                if (element.BlockEvents)
                {
                    mouseEventHandled = true;
                }
            }
            else if (_pointerEnterElement == element)
            {
                _pointerEnterElement = null;
                _pointerExitEvents.Clear();
                element.GetComponents(ref _pointerExitEvents);

                foreach (var evt in _pointerExitEvents)
                {
                    if ((evt as Component).IsEnabled)
                    {
                        evt.OnPointerExit(eventData);
                    }
                }
            }

            if (_dragElement == element && Input.GetMouse(MouseButton.Left))
            {
                _pointerDragEvents.Clear();
                element.GetComponents(ref _pointerDragEvents);

                foreach (var evt in _pointerDragEvents)
                {
                    if ((evt as Component).IsEnabled)
                    {
                        evt.OnPointerDrag(eventData);
                    }
                }
            }
        }


        private void InvokePointerEvent<T>(UIElement element, List<T> events, PointerEventData eventData, Action<PointerEventData> invoker) where T: class, IPointerEvent
        {
            events.Clear();
            element.GetComponents(ref events);

            foreach (var evt in events)
            {
                if ((evt as Component).IsEnabled)
                {
                    invoker(eventData);
                }
            }
        }
        private void DrawRecursive(UIElement element, RectTransform parent)
        {
            if (!element || !element.IsEnabled || !element.Actor.IsActiveSelf)
                return;

            element.RectTransform.Recalculate(parent);

            if (element is UIGraphicsElement graphics)
            {
                graphics.OnCanvasDraw(this);
            }

            foreach (var child in element.Transform.Children)
            {
                if (child.IsEnabled && child.Actor.IsActiveSelf)
                {
                    DrawRecursive(child.GetComponent<UIElement>(), element.RectTransform);
                }
            }
        }

        public void OnUpdate()
        {
            _mouseCanvasPos = ScreenToCanvas(Input.MousePosition);

            _mousePosDelta = _mouseCanvasPos - _prevMousePos;
            _prevMousePos = _mouseCanvasPos;
        }


        public void OnLateUpdate()
        {

            for (int i = Transform.Children.Count - 1; i >= 0; --i)
            {
                bool blocked = false;
                var element = Transform.Children[i].GetComponent<UIElement>();
                EventRecursive(element, RectTransform, ref blocked);
                DrawRecursive(element, RectTransform);
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