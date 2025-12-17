using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public abstract class Gamepad
    {
        public event Action<bool> OnConnected;
        public string Name { get; internal set; }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            internal set
            {
                if (_isConnected == value)
                    return;

                _isConnected = value;
                if (_isConnected)
                {
                    OnConnected?.Invoke(true);
                }
                else
                {
                    OnConnected?.Invoke(false);
                }
            }
        }

        public abstract InputState GetButtonState(GamePadButton button);

        /// <summary>
        /// Gets the value of the specified axis.
        /// </summary>
        /// <param name="axis">The axis to retrieve the value of.</param>
        /// <returns>The axis value, in the range of -1.0 and 1.0 inclusive.</returns>
        public abstract float GetAxis(GamePadAxis axis);
    }

    internal class GamepadStandalone : Gamepad
    {
        internal GLFW.GamePadState State;

        public struct ButtonState
        {
            public bool IsReleased;
            public bool IsDown;
        }

        private Dictionary<GamePadButton, ButtonState> _buttonPending;
        private Dictionary<GamePadButton, ButtonState> _buttonSent;

        public GamepadStandalone()
        {
            _buttonSent = new();
            _buttonPending = new();
            var max = Enum.GetValues(typeof(GamePadButton)).Cast<GamePadButton>().Max() + 1;

            for (int i = 0; i < (int)max; i++)
            {
                _buttonSent.Add((GamePadButton)i, default);
                _buttonPending.Add((GamePadButton)i, default);
            }
        }

        public override InputState GetButtonState(GamePadButton button)
        {
            if (!IsConnected || button == GamePadButton.None)
                return InputState.None;

            var state = (InputState)(int)State.GetButtonState((GLFW.GamePadButton)(int)button);

            var pendingState = _buttonPending[button];
            var sendState = _buttonSent[button];

            if (state == InputState.Press)
            {
                if (!pendingState.IsDown)
                {
                    pendingState.IsDown = true;
                    state = InputState.Down;
                }
                else if (sendState.IsDown)
                {
                    state = InputState.Press;
                }
            }
            else
            {
                pendingState.IsDown = false;
            }

            if (state == InputState.Release)
            {
                if (!pendingState.IsReleased)
                {
                    pendingState.IsReleased = true;
                }
                else if (sendState.IsReleased)
                {
                    state = InputState.None;
                }
            }
            else
            {
                pendingState.IsReleased = false;
            }

            _buttonPending[button] = pendingState;

            return state;
        }

        public override float GetAxis(GamePadAxis axis)
        {
            if (!IsConnected || axis == GamePadAxis.None)
                return 0;

            return State.GetAxis((GLFW.GamePadAxis)(int)axis);
        }

        internal void OnUpdate()
        {
            foreach (var (k, v) in _buttonPending)
            {
                _buttonSent[k] = v;
            }
        }
    }

    public sealed class GamepadInput
    {
        public Gamepad[] Gamepads { get; }
        public Gamepad Main => Gamepads?[0];
        public int ConnectedCount { get; internal set; }

        public event Action<Gamepad> OnGamepadChanged;
        public event Action<Gamepad> OnGamepadDisconnected;

        internal void OnGamePadChanged(Gamepad gamepad)
        {
            OnGamepadChanged?.Invoke(gamepad);
        }

        public GamepadInput(Gamepad[] gamepads)
        {
            Gamepads = gamepads;
        }
    }
}
