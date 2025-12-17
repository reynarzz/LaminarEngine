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
}