using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.GUI
{
    // Note: Here to remember to build a UI system.
    public class CanvasUI : UIElement, IUpdatableComponent
    {
        private Camera _internalCamera;
        public Camera Camera { get; set; }
        private List<UIElement> _elements;

        public void OnUpdate()
        {
        }
    }
}