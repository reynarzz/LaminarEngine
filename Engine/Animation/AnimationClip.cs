using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class AnimationClip : AssetResourceBase
    {
        [ShowFieldNoSerialize]
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

                var max1 = MathF.Max(GetMax(_floatCurves), GetMax(_spriteCurves));
                return MathF.Max(MathF.Max(MathF.Max(MathF.Max(MathF.Max(max1, GetMax(_vec2Curves)), GetMax(_vec3Curves)),
                                 GetMax(_quatCurves)), GetMax(_colorCurves)), _eventCurve.Duration);
            }
        }

        [SerializedField] public bool Loop { get; set; }
        [SerializedField] private Dictionary<string, AnimationCurveBase<float>> _floatCurves = new();
        [SerializedField] private Dictionary<string, AnimationCurveBase<vec2>> _vec2Curves = new();
        [SerializedField] private Dictionary<string, AnimationCurveBase<vec3>> _vec3Curves = new();
        [SerializedField] private Dictionary<string, AnimationCurveBase<quat>> _quatCurves = new();
        [SerializedField] private Dictionary<string, AnimationCurveBase<Color>> _colorCurves = new();
        [SerializedField] private Dictionary<string, AnimationCurveBase<Sprite>> _spriteCurves = new();
        //[SerializedField] private Dictionary<string, AnimationCurveBase> _curves = new();

        [SerializedField] private EventCurve _eventCurve = new();

        // The serializer needs this.
        private AnimationClip() : base(string.Empty, Guid.NewGuid()) // TODO: animation clip
        {
        }

        public AnimationClip(string path, Guid guid) : base(path, guid)
        {
            
        }

        public AnimationClip(string name, bool loop = true) : base(string.Empty, Guid.NewGuid()) // TODO: animation clip
        {
            Name = name;
            Loop = loop;
        }

        //public void AddCurve(string property, AnimationCurveBase curve)
        //{
        //    _curves[property] = curve;
        //}

        public void AddCurve(string property, AnimationCurveBase<float> curve)
        {
            _floatCurves[property] = curve;
        }

        public void AddCurve(string property, AnimationCurveBase<vec2> curve)
        {
            _vec2Curves[property] = curve;
        }

        public void AddCurve(string property, AnimationCurveBase<vec3> curve)
        {
            _vec3Curves[property] = curve;
        }

        public void AddCurve(string property, AnimationCurveBase<quat> curve)
        {
            _quatCurves[property] = curve;
        }
        public void AddCurve(string property, AnimationCurveBase<Color> curve)
        {
            _colorCurves[property] = curve;
        }

        public void AddCurve(string property, AnimationCurveBase<Sprite> curve)
        {
            _spriteCurves[property] = curve;
        }

        public void AddEvent(float time, Action callback)
        {
            _eventCurve.AddKeyFrame(time, callback);
        }

        internal EventCurve GetEventCurve()
        {
            return _eventCurve;
        }

        internal void Evaluate(string property, float time, ref CurveEvaluatedResult result)
        {
            // TODO: 
            // result = _curves.TryGetValue(property, out var c) ? c.Evaluate(time) : default;
        }

        internal float EvaluateFloat(string property, float time)
        {
            return Evaluate(property, time, _floatCurves);
        }

        internal Sprite EvaluateSprite(string property, float time)
        {
            return Evaluate(property, time, _spriteCurves);
        }

        internal vec2 EvaluateVec2(string property, float time)
        {
            return Evaluate(property, time, _vec2Curves);
        }

        internal vec3 EvaluateVec3(string property, float time)
        {
            return Evaluate(property, time, _vec3Curves);
        }

        internal quat EvaluateQuat(string property, float time)
        {
            return Evaluate(property, time, _quatCurves);
        }

        internal Color EvaluateColor(string property, float time)
        {
            return Evaluate(property, time, _colorCurves);
        }

        internal void EvaluateEvent(float time)
        {
            _eventCurve?.Evaluate(time);
        }

        private T Evaluate<T>(string property, float time, Dictionary<string, AnimationCurveBase<T>> curves)
        {
            return curves.TryGetValue(property, out var c) ? c.Evaluate(time) : default;
        }

        public bool HasPropertyAnyCurve(string property)
        {
            if (HasPropertyInFloatCurves(property))
                return true;

            if (HasPropertyInSpriteCurves(property))
                return true;

            if (HasPropertyInVec2Curves(property))
                return true;

            if (HasPropertyInVec3Curves(property))
                return true;

            if (HasPropertyInQuatCurves(property))
                return true;

            if (HasPropertyInColorCurves(property))
                return true;

            return false;
        }

        public bool HasProperty<T>(string property, Dictionary<string, AnimationCurveBase<T>> curve)
        {
            return curve.ContainsKey(property);
        }
        public bool HasPropertyInFloatCurves(string property)
        {
            return HasProperty(property, _floatCurves);
        }
        public bool HasPropertyInSpriteCurves(string property)
        {
            return HasProperty(property, _spriteCurves);
        }
        public bool HasPropertyInVec2Curves(string property)
        {
            return HasProperty(property, _vec2Curves);
        }
        public bool HasPropertyInVec3Curves(string property)
        {
            return HasProperty(property, _vec3Curves);
        }
        public bool HasPropertyInQuatCurves(string property)
        {
            return HasProperty(property, _quatCurves);
        }
        public bool HasPropertyInColorCurves(string property)
        {
            return HasProperty(property, _colorCurves);
        }

    }
}
