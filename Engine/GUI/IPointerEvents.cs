using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    public interface IPointerDownEvent
    {
        void OnPointerDown(vec2 mousePos);
    }

    public interface IPointerUpEvent
    {
        void OnPointerUp(vec2 mousePos);
    }

    public interface IPointerHoldEvent
    {
        void OnPointerHold(vec2 mousePos);
    }

    public interface IPointerHoverEvent
    {
        void OnPointerHover(vec2 mousePos);
    }
}
