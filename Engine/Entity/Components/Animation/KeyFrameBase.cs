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
        public float InTangent { get; }
        public float OutTangent { get; }

        public KeyFrameBase(float time, float inTangent = 0f, float outTangent = 0f)
        {
            Time = time;
            InTangent = inTangent;
            OutTangent = outTangent;
        }
    }

    public class KeyFrameBase<T> : KeyFrameBase
    {
        public T Value { get; }

        public KeyFrameBase(float time, T value, float inTangent = 0f, float outTangent = 0f) 
            : base(time, inTangent, outTangent)
        {
            Value = value;
        }
    }
}
