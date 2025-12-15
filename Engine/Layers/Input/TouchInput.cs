using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public enum TouchEvent
    {
        None = 0,
        Down = 1,
        Up = 2,
        Move = 3,
        Cancel = 4,
    }

    public struct TouchState
    {
        public int PointerId;
        public TouchEvent Type;
        public vec2 Position;
    }

    public class TouchInput
    {
        public int TouchCount { get; internal set; } = 0;
        internal TouchState[] State = new TouchState[20];

        public ref TouchState GetTouch(int index)
        {
            return ref State[index];
        }
    }

}
