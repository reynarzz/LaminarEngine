using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface IKeyFrame
    {
        public float Time { get; }
    }

    public interface IKeyFrame<T> : IKeyFrame
    {
        public T Value { get; }
    }
    public struct Keyframe<T> : IKeyFrame<T>
    {
        public T Value { get; }
        public float Time { get; }
        public Keyframe(float time, T value)
        {
            Time = time;
            Value = value;
        }
    }
    public struct KeyFrameHermite<T> : IKeyFrame<T>
    {
        public T InTangent { get; set; }
        public T OutTangent { get; set; }

        public T Value { get; }
        public float Time { get; }

        public KeyFrameHermite(float time, T value) : this(time, value, default, default)
        {
        }

        public KeyFrameHermite(float time, T value, T inTangent, T outTangent)
        {
            Time = time;
            Value = value;
            InTangent = inTangent;
            OutTangent = outTangent;
        }
    }
}
