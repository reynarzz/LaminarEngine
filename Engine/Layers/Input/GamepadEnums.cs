using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum GamepadId
    {
        Gamepad1,
        Gamepad2,
        Gamepad3,
        Gamepad4,
        Gamepad5,
        Gamepad6,
        Gamepad7,
        Gamepad8,
        Gamepad9,
        Gamepad10,
        Gamepad11,
        Gamepad12,
        Gamepad13,
        Gamepad14,
        Gamepad15,
        Gamepad16
    }

    public enum InputState 
    {
        None = -1,
        Release,
        Press,
        Repeat,
        Down
    }

    public enum GamePadButton 
    {
        None = -1,
        A = 0,
        B = 1,
        X = 2,
        Y = 3,
        LeftBumper = 4,
        RightBumper = 5,
        Back = 6,
        Start = 7,
        Guide = 8,
        LeftThumb = 9,
        RightThumb = 10,
        DpadUp = 11,
        DpadRight = 12,
        DpadDown = 13,
        DpadLeft = 14,
        //Cross = 0,
        //Circle = 1,
        //Square = 2,
        //Triangle = 3
    }

    public enum GamePadAxis
    {
        None = -1,
        LeftX,
        LeftY,
        RightX,
        RightY,
        LeftTrigger,
        RightTrigger
    }
}
