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
        public vec2 Position { get; }
        public vec2 Delta { get;}
        public vec2 NormalizedPosition { get; }

        /// <summary>
        /// Actor who received the event.
        /// </summary>
        public Actor Actor { get; }
        public UICanvas Canvas { get; }
        public PointerEventData(Actor actor, UICanvas canvas, vec2 position, vec2 delta, vec2 positionNormalized)
        {
            Actor = actor;
            Canvas = canvas;
            Position = position;
            Delta = delta;
            NormalizedPosition = positionNormalized;
        }
    }

    [RequiredComponent(typeof(RectTransform))]
    public class UICanvas : Component, ILateUpdatableComponent, IUpdatableComponent
    {
        public RectTransform RectTransform { get; private set; }
        private vec2 _prevMousePos;
        private vec2 _mouseCanvasPos;
        private vec2 _mouseDeltaPos;
        private vec2 _mouseNormalizedPosition;
        private List<IPointerEnterEvent> _pointerEnterEvents = new();
        private List<IPointerExitEvent> _pointerExitEvents = new();
        private List<IPointerDownEvent> _pointerDownEvents = new();
        private List<IPointerUpEvent> _pointerUpEvents = new();
        private List<IPointerDragEvent> _pointerDragEvents = new();

        // Note: this only works for single cursor, probably I will not implement multi-touch with finger id etc...
        private UIElement _dragElement;
        private UIElement _pointerEnterElement;
        private readonly Action<IPointerEnterEvent, PointerEventData> _pointerEnterEventInvoker = (p, d) => p.OnPointerEnter(d);
        private readonly Action<IPointerExitEvent, PointerEventData> _pointerExitEventInvoker = (p, d) => p.OnPointerExit(d);
        private readonly Action<IPointerDownEvent, PointerEventData> _pointerDownEventInvoker = (p, d) => p.OnPointerDown(d);
        private readonly Action<IPointerUpEvent, PointerEventData> _pointerUpEventInvoker = (p, d) => p.OnPointerUp(d);
        private readonly Action<IPointerDragEvent, PointerEventData> _pointerDragEventInvoker = (p, d) => p.OnPointerDrag(d);

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
      

            var eventData = new PointerEventData(element.Actor, this, _mouseCanvasPos, _mouseDeltaPos, _mouseNormalizedPosition);

            if (hasMouse && element.ReceiveEvents && !mouseEventHandled)
            {
                // Pointer down event
                if (Input.GetMouseDown(MouseButton.Left))
                {
                    _dragElement = element;
                    InvokePointerEvent(element, _pointerDownEvents, eventData, _pointerDownEventInvoker);
                }
                // Pointer up event
                if (Input.GetMouseUp(MouseButton.Left))
                {
                    _dragElement = null;
                    InvokePointerEvent(element, _pointerUpEvents, eventData, _pointerUpEventInvoker);
                }

                // Pointer enter event
                if (_pointerEnterElement != element)
                {
                    _pointerEnterElement = element;
                    InvokePointerEvent(element, _pointerEnterEvents, eventData, _pointerEnterEventInvoker);
                }

                if (element.BlockEvents)
                {
                    mouseEventHandled = true;
                }
            }
            else if (_pointerEnterElement == element) // Pointer exit event
            {
                _pointerEnterElement = null;
                InvokePointerEvent(element, _pointerExitEvents, eventData, _pointerExitEventInvoker);
            }

            // Drag event
            if (_dragElement == element && Input.GetMouse(MouseButton.Left))
            {
                InvokePointerEvent(element, _pointerDragEvents, eventData, _pointerDragEventInvoker);
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
            _mouseDeltaPos = _mouseCanvasPos - _prevMousePos;
            _mouseNormalizedPosition = _mouseCanvasPos / RectTransform.Rect.Size;
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

        private void InvokePointerEvent<T>(UIElement element, List<T> events,
                                           PointerEventData eventData, Action<T, PointerEventData> invoker)
                                           where T : class, IComponent, IPointerEvent
        {
            events.Clear();
            //element.GetComponentsInChildren(ref events);
            element.GetComponents(ref events);

            foreach (var evt in events)
            {
                if (evt.IsEnabled)
                {
                    invoker(evt, eventData);
                }
            }
        }
    }
}