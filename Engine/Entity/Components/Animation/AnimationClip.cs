using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimationClip : EObject
    {
        public float Duration
        {
            get 
            {
                float GetMax<T>(Dictionary<string, AnimationCurveBase<T>> dict)
                {
                    float max = float.MinValue;

                    foreach (var (key, value) in dict)
                    {
                        if (max < value.Duration)
                        {
                            max = value.Duration;
                        }
                    }

                    return max;
                }

                return MathF.Max(GetMax(_floatCurves), GetMax(_spriteCurves));
            }
        }

        public bool Loop { get; set; }
        private Dictionary<string, AnimationCurveBase<float>> _floatCurves = new();
        private Dictionary<string, AnimationCurveBase<Sprite>> _spriteCurves = new();
        private EventCurve _eventCurve = new();

        public AnimationClip(string name, bool loop = true)
        {
            Name = name;
            Loop = loop;
        }

        public void AddCurve(string property, AnimationCurveBase<float> curve)
        {
            _floatCurves[property] = curve;
        }

        public void AddCurve(string property, AnimationCurveBase<Sprite> curve)
        {
            _spriteCurves[property] = curve;
        }

        public void AddEvent(EventCurve curve)
        {
            _eventCurve = curve;
        }

        internal float EvaluateFloat(string property, float time)
        {
            return Evaluate(property, time, _floatCurves);
        }

        internal Sprite EvaluateSprite(string property, float time)
        {
            return Evaluate(property, time, _spriteCurves);
        }

        internal Action EvaluateEvent(float time)
        {
            if (_eventCurve == null)
                return null;

            return _eventCurve.EvaluateTime(time);
        }

        private T Evaluate<T>(string property, float time, Dictionary<string, AnimationCurveBase<T>> curves)
        {
            return curves.TryGetValue(property, out var c) ? c.EvaluateTime(time) : default;
        }
    }
}
