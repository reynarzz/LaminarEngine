using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.IOS
{
    internal class InputLayerIOS : InputLayerBase
    {
        public override TouchInput Touch { get; } = new();
        public override GamepadInput Gamepad { get; } = new();
        public override vec2 MousePosition { get; } = default;
        public override bool GetKey(KeyCode key) {return false; }
        public override bool GetKeyDown(KeyCode key){return false; }
        public override bool GetKeyUp(KeyCode key){return false; }
        public override bool GetMouse(MouseButton button){return false; }
        public override bool GetMouseDown(MouseButton button){return false; }
        public override bool GetMouseUp(MouseButton button){return false; }
        public override void Close()
        {
            
        }
    }
}