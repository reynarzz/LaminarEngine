using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class KeyFrameBase
    {
        public float Time { get; }

        public KeyFrameBase(float time)
        {
            Time = time;
        }
    }

    public class KeyFrameBase<T> : KeyFrameBase
    {
        public T Value { get; }

        public KeyFrameBase(float time, T value) : base(time)
        {
            Value = value;
        }
    }

    public class KeyFrameHermite<T> : KeyFrameBase<T>
    {
        public T InTangent { get; set; }
        public T OutTangent { get; set; }

        public KeyFrameHermite(float time, T value, T inTangent, T outTangent)
            : base(time, value)
        {
            InTangent = inTangent;
            OutTangent = outTangent;
        }
    }
}
