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
        Stationary,
        Up,
        Move,
    }

    public struct TouchState
    {
        public int PointerId;
        public TouchEvent Type;
        public vec2 Position;
        public vec2 Delta;
        internal bool IsDownEventConsumed;
        internal bool IsUpEventConsumed;
        internal vec2 PrevPosition;
        internal long LastMoveTimeMs;
    }

    public class TouchInput
    {
        internal const int MaxTouches = 20;
        internal const float DEATH_ZONE = 0.001f;
        
        public int TouchCount { get; internal set; } = 0;
        internal TouchState[] State = new TouchState[MaxTouches];

        public ref TouchState GetTouch(int index)
        {
            return ref State[index];
        }
    }

}
