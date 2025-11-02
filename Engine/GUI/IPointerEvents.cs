using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public interface IPointerEvent : IComponent
    {
    }

    public interface IPointerDownEvent : IPointerEvent
    {
        void OnPointerDown(PointerEventData eventData);
    }

    public interface IPointerUpEvent : IPointerEvent
    {
        void OnPointerUp(PointerEventData eventData);
    }

    public interface IPointerHoldEvent : IPointerEvent
    {
        void OnPointerHold(PointerEventData eventData);
    }
    public interface IPointerDragEvent : IPointerEvent
    {
        void OnPointerDrag(PointerEventData eventData);
    }
    public interface IPointerEnterEvent : IPointerEvent
    {
        void OnPointerEnter(PointerEventData eventData);
    }

    public interface IPointerExitEvent : IPointerEvent
    {
        void OnPointerExit(PointerEventData eventData);
    }
}
