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
        public static readonly TouchState None = new()
        {
            PointerId = -1,
            Type = TouchEvent.None,
            Position = vec2.Zero,
            Delta = vec2.Zero,
            IsDownEventConsumed = false,
            IsUpEventConsumed = false,
            PrevPosition = vec2.Zero,
            LastMoveTimeMs = 0
        };

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
        internal readonly TouchState[] State;

        public TouchInput()
        {
            State = new TouchState[MaxTouches];
            for (int i = 0; i < State.Length; i++)
            {
                State[i] = TouchState.None;
            }
        }
        
        public TouchState GetTouch(int index)
        {
            if (TouchCount <= 0)
                return TouchState.None;

            for (int i = 0; i < MaxTouches; i++)
            {
                ref var state = ref State[index + i];

                if (state.PointerId >= 0)
                {
                    return state;
                }
            }

            return TouchState.None;
        }
    }
}