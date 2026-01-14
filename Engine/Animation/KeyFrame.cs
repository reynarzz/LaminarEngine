using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public interface IKeyFrame
    {
        [SerializedField] public float Time { get; set; }
    }

    public interface IKeyFrame<T> : IKeyFrame
    {
        public T Value { get; set; }
    }
    public struct Keyframe<T> : IKeyFrame<T>
    {
        [SerializedField] public T Value { get; set; }
        [SerializedField] public float Time { get; set; }
        public Keyframe(float time, T value)
        {
            Time = time;
            Value = value;
        }
    }
    public struct KeyFrameHermite<T> : IKeyFrame<T>
    {
        [SerializedField] public T InTangent { get; set; }
        [SerializedField] public T OutTangent { get; set; }

        [SerializedField] public T Value { get; set; }
        [SerializedField] public float Time { get; set; }

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
