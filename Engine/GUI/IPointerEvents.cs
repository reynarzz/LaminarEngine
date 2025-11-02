using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public interface IPointerEvent 
    {
    }

    public interface IPointerDownEvent : IPointerEvent
    {
        void OnPointerDown(vec2 mousePos);
    }

    public interface IPointerUpEvent : IPointerEvent
    {
        void OnPointerUp(vec2 mousePos);
    }

    public interface IPointerHoldEvent : IPointerEvent
    {
        void OnPointerHold(vec2 mousePos);
    }

    public interface IPointerHoverEvent : IPointerEvent
    {
        void OnPointerHover(vec2 mousePos);
    }
}
