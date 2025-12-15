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
        None,
        Down,
        Pressed,
        Up,
        Move,
        Cancel,
    }

    public struct TouchState
    {
        public int PointerId;
        public TouchEvent Type;
        public vec2 Position;
        internal bool IsDownEventConsumed;
        internal bool IsUpEventConsumed;
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
