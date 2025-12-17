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
        private Dictionary<GamePadButton, bool> _releasePending;
        private Dictionary<GamePadButton, bool> _releaseSent;
        public GamepadStandalone()
        {
            _releaseSent = new Dictionary<GamePadButton, bool>();
            _releasePending = new Dictionary<GamePadButton, bool>();
            var max = Enum.GetValues(typeof(GamePadButton)).Cast<GamePadButton>().Max() + 1;

            for (int i = 0; i < (int)max; i++)
            {
                _releaseSent.Add((GamePadButton)i, false);
                _releasePending.Add((GamePadButton)i, false);
            }
        }

        public override InputState GetButtonState(GamePadButton button)
        {
            if (!IsConnected || button == GamePadButton.None)
                return InputState.None;
            var glfwButton = (GLFW.GamePadButton)(int)button;

            var value = (InputState)(int)State.GetButtonState(glfwButton);

            if (value == InputState.Release)
            {
                if (!_releasePending[button])
                {
                    _releasePending[button] = true;
                }
                else if (_releaseSent[button])
                {
                    value = InputState.None;
                }
            }
            else 
            {
                _releasePending[button] = false;
            }

            return value;
        }

        public override float GetAxis(GamePadAxis axis)
        {
            if (!IsConnected || axis == GamePadAxis.None)
                return 0;

            return State.GetAxis((GLFW.GamePadAxis)(int)axis);
        }

        internal void OnUpdate()
        {
            foreach (var (k, v) in _releasePending)
            {
                _releaseSent[k] = v;
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
