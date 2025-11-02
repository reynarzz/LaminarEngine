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
    internal class Test_UIEvent : ScriptBehavior, IDragEvent
    {
        public void OnPointerDrag(PointerEventData eventData)
        {
            Transform.LocalPosition += new vec3(eventData.Delta);
            Debug.Log("Drag: " + Name + "Delta: " + new vec3(eventData.Delta));
        }
    }
}