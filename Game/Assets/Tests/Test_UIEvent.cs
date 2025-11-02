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
    internal class Test_UIEvent : ScriptBehavior, IDragEvent, IPointerEnterEvent, IPointerExitEvent
    {
        public void OnPointerDrag(PointerEventData eventData)
        {
            Transform.LocalPosition += new vec3(eventData.Delta);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Pointer enter: " + Name);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Pointer exit: " + Name);
        }
    }
}