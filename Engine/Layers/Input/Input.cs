using Engine.Layers.Input;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Input
    {
        internal static InputLayerBase CurrentInput { get; set; }
      
        public static TouchInput Touch => CurrentInput.Touch;
        public static vec2 MousePosition => CurrentInput.MousePosition;

        public static bool GetKey(KeyCode key)
        {
            return CurrentInput.GetKey(key);
        }
        public static bool GetKeyDown(KeyCode key)
        {
            return CurrentInput.GetKeyDown(key);
        }
        public static bool GetKeyUp(KeyCode key)
        {
            return CurrentInput.GetKeyUp(key);
        }
        public static bool GetMouse(MouseButton button)
        {
            return CurrentInput.GetMouse(button);
        }
        public static bool GetMouseDown(MouseButton button)
        {
            return CurrentInput.GetMouseDown(button);
        }
        public static bool GetMouseUp(MouseButton button) 
        {
            return CurrentInput.GetMouseUp(button);
        }
    }
}
