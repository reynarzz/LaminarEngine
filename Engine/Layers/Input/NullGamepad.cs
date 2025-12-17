using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Layers.Input
{
    internal class NullGamepad : Gamepad
    {
        public override float GetAxis(GamePadAxis axis)
        {
            return 0;
        }

        public override InputState GetButtonState(GamePadButton button)
        {
            return InputState.None;
        }
    }
}
