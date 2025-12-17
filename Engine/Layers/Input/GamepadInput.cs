using Engine.Layers.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public sealed class GamepadInput
    {
        private readonly Gamepad _nullGamepad;

        private Gamepad[] _gamepads;
        public Gamepad Main => _gamepads?[0] ?? _nullGamepad;
        public int ConnectedCount { get; internal set; } = 0;

        public event Action<Gamepad> OnGamepadChanged;
        public event Action<Gamepad> OnGamepadDisconnected;

        public GamepadInput()
        {
            _nullGamepad = new NullGamepad()
            {
                Name = "Null Gamepad",
                IsConnected = false
            };
        }

        public GamepadInput(Gamepad[] gamepads)
        {
            _gamepads = gamepads;
        }

        public Gamepad GetGamepad(int index)
        {
            return _gamepads?[index] ?? _nullGamepad;
        }

        internal void OnGamePadChanged(Gamepad gamepad)
        {
            OnGamepadChanged?.Invoke(gamepad);
        }
    }
}
