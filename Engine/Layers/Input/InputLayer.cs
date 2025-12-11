using System;
using System.Collections.Generic;
using Engine;
using Engine.Layers;
using GlmNet;

public enum KeyCode
{
    Space = 32,
    Apostrophe = 39,
    Comma = 44,
    Minus = 45,
    Period = 46,
    Slash = 47,
    Alpha0 = 48,
    Alpha1 = 49,
    Alpha2 = 50,
    Alpha3 = 51,
    Alpha4 = 52,
    Alpha5 = 53,
    Alpha6 = 54,
    Alpha7 = 55,
    Alpha8 = 56,
    Alpha9 = 57,
    SemiColon = 59,
    Equal = 61,
    A = 65,
    B = 66,
    C = 67,
    D = 68,
    E = 69,
    F = 70,
    G = 71,
    H = 72,
    I = 73,
    J = 74,
    K = 75,
    L = 76,
    M = 77,
    N = 78,
    O = 79,
    P = 80,
    Q = 81,
    R = 82,
    S = 83,
    T = 84,
    U = 85,
    V = 86,
    W = 87,
    X = 88,
    Y = 89,
    Z = 90,
    LeftBracket = 91,
    Backslash = 92,
    RightBracket = 93,
    GraveAccent = 96,
    World1 = 161,
    World2 = 162,
    Escape = 256,
    Enter = 257,
    Tab = 258,
    Backspace = 259,
    Insert = 260,
    Delete = 261,
    Right = 262,
    Left = 263,
    Down = 264,
    Up = 265,
    PageUp = 266,
    PageDown = 267,
    Home = 268,
    End = 269,
    CapsLock = 280,
    ScrollLock = 281,
    NumLock = 282,
    PrintScreen = 283,
    Pause = 284,
    F1 = 290,
    F2 = 291,
    F3 = 292,
    F4 = 293,
    F5 = 294,
    F6 = 295,
    F7 = 296,
    F8 = 297,
    F9 = 298,
    F10 = 299,
    F11 = 300,
    F12 = 301,
    F13 = 302,
    F14 = 303,
    F15 = 304,
    F16 = 305,
    F17 = 306,
    F18 = 307,
    F19 = 308,
    F20 = 309,
    F21 = 310,
    F22 = 311,
    F23 = 312,
    F24 = 313,
    F25 = 314,
    Numpad0 = 320,
    Numpad1 = 321,
    Numpad2 = 322,
    Numpad3 = 323,
    Numpad4 = 324,
    Numpad5 = 325,
    Numpad6 = 326,
    Numpad7 = 327,
    Numpad8 = 328,
    Numpad9 = 329,
    NumpadDecimal = 330,
    NumpadDivide = 331,
    NumpadMultiply = 332,
    NumpadSubtract = 333,
    NumpadAdd = 334,
    NumpadEnter = 335,
    NumpadEqual = 336,
    LeftShift = 340,
    LeftControl = 341,
    LeftAlt = 342,
    LeftSuper = 343,
    RightShift = 344,
    RightControl = 345,
    RightAlt = 346,
    RightSuper = 347,
    Menu = 348
}

public enum MouseButton
{
    Left = 0,
    Right = 1,
    Middle = 2
}

public class Input : LayerBase
{
    private static HashSet<KeyCode> _currentKeys = new();
    private static HashSet<KeyCode> _previousKeys = new();

    private static HashSet<MouseButton> _currentMouse = new();
    private static HashSet<MouseButton> _previousMouse = new();

    public static vec2 MousePosition { get; internal set; }

    private KeyCode[] _keyCodesArray;
    private MouseButton[] _mouseButtonsArray;
    public override void Initialize() 
    {
        _keyCodesArray = Enum.GetValues<KeyCode>();
        _mouseButtonsArray = Enum.GetValues<MouseButton>();

        _previousKeys = new HashSet<KeyCode>();
        _previousMouse = new HashSet<MouseButton>();
    }

    private void CopyKeys<T>(HashSet<T> from, HashSet<T> to)
    {
        to.Clear();
        foreach (var key in from) 
        {
            to.Add(key);
        }
    }

    internal override void UpdateLayer()
    {

        CopyKeys(_currentKeys, _previousKeys);
        CopyKeys(_currentMouse, _previousMouse);


#if DESKTOP
        // Poll OS events
        GLFW.Glfw.PollEvents();

        // Update current key states
        foreach (KeyCode key in _keyCodesArray)
        {
            bool down = GLFW.Glfw.GetKey(Engine.Window.NativeWindow, (GLFW.Keys)key) == GLFW.InputState.Press;

            if (down)
            {
                _currentKeys.Add(key);
            }
            else
            {
                _currentKeys.Remove(key);
            }
        }

        // Update current mouse button states
        foreach (MouseButton button in _mouseButtonsArray)
        {
            bool down = GLFW.Glfw.GetMouseButton(Engine.Window.NativeWindow, (GLFW.MouseButton)button) == GLFW.InputState.Press;

            if (down)
            {
                _currentMouse.Add(button);
            }
            else
            {
                _currentMouse.Remove(button);
            }
        }

        // Update mouse position
        GLFW.Glfw.GetCursorPosition(Engine.Window.NativeWindow, out double x, out double y);
        MousePosition = new vec2((float)x, (float)y);
#endif
    }

    public static bool GetKey(KeyCode key)
    {
        return _currentKeys.Contains(key);
    }

    public static bool GetKeyDown(KeyCode key)
    {
        return _currentKeys.Contains(key) &&
               (!_previousKeys.Contains(key));
    }

    public static bool GetKeyUp(KeyCode key)
    {
        return _previousKeys.Contains(key) &&
               (!_currentKeys.Contains(key));
    }

    public static bool GetMouse(MouseButton button)
    {
        return _currentMouse.Contains(button);
    }

    public static bool GetMouseDown(MouseButton button)
    {
        return _currentMouse.Contains(button)&&
               (!_previousMouse.Contains(button));
    }

    public static bool GetMouseUp(MouseButton button)
    {
        return _previousMouse.Contains(button) &&
               (!_currentMouse.Contains(button));
    }

    public override void Close()
    {
    }
}
