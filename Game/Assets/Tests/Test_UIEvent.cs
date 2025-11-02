using Engine;
using Engine.GUI;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    internal class Test_UIEvent : ScriptBehavior, IPointerDownEvent, IPointerUpEvent, IPointerDragEvent, IPointerEnterEvent, IPointerExitEvent
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer down: " + Name);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Pointer up: " + Name);
        }
        public void OnPointerDrag(PointerEventData eventData)
        {
            Transform.LocalPosition += new vec3(eventData.Delta);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer enter: " + eventData.Actor.Name);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Pointer exit: " + eventData.Actor.Name);
        }

    }
}