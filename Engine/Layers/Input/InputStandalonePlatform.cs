#if DESKTOP
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Engine;
using GLFW;
using GlmNet;

namespace Engine.Layers.Input
{
    public class InputStandAlonePlatform : InputLayerBase
    {
        private HashSet<KeyCode> _currentKeys = new();
        private HashSet<KeyCode> _previousKeys = new();

        private HashSet<MouseButton> _currentMouse = new();
        private HashSet<MouseButton> _previousMouse = new();

        private vec2 _mousePos;
        public override vec2 MousePosition => _mousePos;

        public override TouchInput Touch { get; } = new TouchInput();
        private GamepadStandalone[] _gamepads;
        public override GamepadInput Gamepad { get; }

        private KeyCode[] _keyCodesArray;
        private MouseButton[] _mouseButtonsArray;
        private const int MAX_GAMEPAD_IDS = 12;//(int)GamepadId.Gamepad16;

        public InputStandAlonePlatform()
        {
            _gamepads = new GamepadStandalone[MAX_GAMEPAD_IDS];
            for (int i = 0; i < _gamepads.Length; i++)
            {
                _gamepads[i] = new GamepadStandalone();
            }
            Gamepad = new GamepadInput(_gamepads);
        }

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

            // Poll OS events
            Glfw.PollEvents();
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
            _mousePos = new vec2((float)x, (float)y);

            GamepadUpdate();
        }

        private void GamepadUpdate()
        {
            int connectedCount = 0;
            for (int i = 0; i < MAX_GAMEPAD_IDS; i++)
            {
                var gamepad = _gamepads[i];
                if (Glfw.GetGamepadState(i, out var state))
                {
                    connectedCount++;
                    var wasDisconnected = !gamepad.IsConnected;
                    gamepad.IsConnected = true;
                    gamepad.State = state;

                    if (wasDisconnected)
                    {
                        gamepad.Name = Glfw.GetGamepadName(i);
                        Gamepad.OnGamePadChanged(gamepad);
                    }
                }
                else
                {
                    var wasConnected = gamepad.IsConnected;
                    gamepad.State = default;
                    gamepad.IsConnected = false;
                    if (wasConnected)
                    {
                        gamepad.OnUpdate();
                        Gamepad.OnGamePadChanged(gamepad);
                    }

                    gamepad.Name = string.Empty;
                }

                if (gamepad.IsConnected)
                {
                    gamepad.OnUpdate();
                }
            }

            Gamepad.ConnectedCount = connectedCount;
        }

        public override bool GetKey(KeyCode key)
        {
            return _currentKeys.Contains(key);
        }

        public override bool GetKeyDown(KeyCode key)
        {
            return _currentKeys.Contains(key) &&
                   (!_previousKeys.Contains(key));
        }

        public override bool GetKeyUp(KeyCode key)
        {
            return _previousKeys.Contains(key) &&
                   (!_currentKeys.Contains(key));
        }

        public override bool GetMouse(MouseButton button)
        {
            return _currentMouse.Contains(button);
        }

        public override bool GetMouseDown(MouseButton button)
        {
            return _currentMouse.Contains(button) &&
                   (!_previousMouse.Contains(button));
        }

        public override bool GetMouseUp(MouseButton button)
        {
            return _previousMouse.Contains(button) &&
                   (!_currentMouse.Contains(button));
        }

        public override void Close()
        {
        }
    }
}
#endif
