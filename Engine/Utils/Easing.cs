using System;
using GlmNet;
using static System.MathF;

namespace Engine
{
    public enum EasingType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseOutOvershoot,
        EaseSpike,
        EaseFlash,
        EaseSmoothStep,
        EaseSmootherStep,
        EasePunch,
        EaseSpring
    }

    public static class Easing
    {
        public static float Linear(float t)
        {
            return t;
        }

        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        public static float EaseOutQuad(float t)
        {
            return t * (2f - t);
        }

        public static float EaseInOutQuad(float t)
        {
            if (t < 0.5f)
                return 2f * t * t;
            return -1f + (4f - 2f * t) * t;
        }

        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }

        public static float EaseOutCubic(float t)
        {
            t -= 1f;
            return t * t * t + 1f;
        }

        public static float EaseInOutCubic(float t)
        {
            if (t < 0.5f)
                return 4f * t * t * t;
            t -= 1f;
            return 1f + 4f * t * t * t;
        }

        public static float EaseInQuart(float t)
        {
            return t * t * t * t;
        }

        public static float EaseOutQuart(float t)
        {
            t -= 1f;
            return 1f - t * t * t * t;
        }

        public static float EaseInOutQuart(float t)
        {
            if (t < 0.5f)
                return 8f * t * t * t * t;
            t -= 1f;
            return 1f - 8f * t * t * t * t;
        }

        public static float EaseInQuint(float t)
        {
            return t * t * t * t * t;
        }

        public static float EaseOutQuint(float t)
        {
            t -= 1f;
            return 1f + t * t * t * t * t;
        }

        public static float EaseInOutQuint(float t)
        {
            if (t < 0.5f)
                return 16f * t * t * t * t * t;
            t -= 1f;
            return 1f + 16f * t * t * t * t * t;
        }

        public static float EaseInSine(float t)
        {
            return 1f - Cos((t * PI) / 2f);
        }

        public static float EaseOutSine(float t)
        {
            return Sin((t * PI) / 2f);
        }

        public static float EaseInOutSine(float t)
        {
            return -(Cos(PI * t) - 1f) / 2f;
        }

        public static float EaseInExpo(float t)
        {
            return Pow(2f, 10f * (t - 1f));
        }

        public static float EaseOutExpo(float t)
        {
            return 1f - Pow(2f, -10f * t);
        }

        public static float EaseInOutExpo(float t)
        {
            if (t < 0.5f)
                return Pow(2f, 10f * (2f * t - 1f)) / 2f;
            return (2f - Pow(2f, -10f * (2f * t - 1f))) / 2f;
        }

        public static float EaseInCirc(float t)
        {
            return 1f - Sqrt(1f - t * t);
        }

        public static float EaseOutCirc(float t)
        {
            t -= 1f;
            return Sqrt(1f - t * t);
        }

        public static float EaseInOutCirc(float t)
        {
            if (t < 0.5f)
                return (1f - Sqrt(1f - 4f * t * t)) / 2f;
            t = 2f * t - 2f;
            return (Sqrt(1f - t * t) + 1f) / 2f;
        }

        public static float EaseInElastic(float t)
        {
            if (t == 0f || t == 1f)
                return t;
            return -Pow(2f, 10f * (t - 1f)) * Sin((t - 1.075f) * (2f * PI) / 0.3f);
        }

        public static float EaseOutElastic(float t)
        {
            if (t == 0f || t == 1f)
                return t;
            return Pow(2f, -10f * t) * Sin((t - 0.075f) * (2f * PI) / 0.3f) + 1f;
        }

        public static float EaseInOutElastic(float t)
        {
            if (t == 0f || t == 1f)
                return t;
            t = t * 2f;
            if (t < 1f)
                return -0.5f * Pow(2f, 10f * (t - 1f)) * Sin((t - 1.1125f) * (2f * PI) / 0.45f);
            return Pow(2f, -10f * (t - 1f)) * Sin((t - 1.1125f) * (2f * PI) / 0.45f) * 0.5f + 1f;
        }

        public static float EaseInBack(float t)
        {
            float s = 1.70158f;
            return t * t * ((s + 1f) * t - s);
        }

        public static float EaseOutBack(float t)
        {
            float s = 1.70158f;
            t -= 1f;
            return t * t * ((s + 1f) * t + s) + 1f;
        }

        public static float EaseInOutBack(float t)
        {
            float s = 1.70158f * 1.525f;
            t *= 2f;
            if (t < 1f)
                return 0.5f * (t * t * ((s + 1f) * t - s));
            t -= 2f;
            return 0.5f * (t * t * ((s + 1f) * t + s) + 2f);
        }

        public static float EaseOutBounce(float t)
        {
            if (t < 1f / 2.75f)
                return 7.5625f * t * t;
            if (t < 2f / 2.75f)
            {
                t -= 1.5f / 2.75f;
                return 7.5625f * t * t + 0.75f;
            }
            if (t < 2.5f / 2.75f)
            {
                t -= 2.25f / 2.75f;
                return 7.5625f * t * t + 0.9375f;
            }
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }

        public static float EaseInBounce(float t)
        {
            return 1f - EaseOutBounce(1f - t);
        }

        public static float EaseInOutBounce(float t)
        {
            if (t < 0.5f)
                return EaseInBounce(t * 2f) * 0.5f;
            return EaseOutBounce(t * 2f - 1f) * 0.5f + 0.5f;
        }

        public static float EaseOutOvershoot(float t)
        {
            float s = 2.5f;
            t -= 1f;
            return 1f + (s + 1f) * t * t * ((s + 1f) * t + s);
        }

        public static float EaseSpike(float t)
        {
            return 4f * t * (1f - t);
        }

        public static float EaseFlash(float t)
        {
            return Pow(Sin(t * PI), 3f);
        }

        public static float EaseSmoothStep(float t)
        {
            return t * t * (3f - 2f * t);
        }

        public static float EaseSmootherStep(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        public static float EasePunch(float t)
        {
            if (t == 0f || t == 1f)
                return 0f;
            float period = 0.3f;
            return Pow(2f, -10f * t) * Sin((t - (period / 4f)) * (2f * PI) / period);
        }

        public static float EaseSpring(float t)
        {
            return (Sin(t * PI * (0.2f + 2.5f * t * t * t)) * Pow(1f - t, 2.2f) + t) * (1f + (1.2f * (1f - t)));
        }


        public static float Apply(EasingType type, float start, float end, float t)
        {
            float eased = Apply(type, t);
            return start + (end - start) * eased;
        }

        public static vec2 Apply(EasingType type, vec2 start, vec2 end, float t)
        {
            float eased = Apply(type, t);
            return start + (end - start) * eased;
        }

        public static vec3 Apply(EasingType type, vec3 start, vec3 end, float t)
        {
            float eased = Apply(type, t);
            return start + (end - start) * eased;
        }

        public static quat Apply(EasingType type, quat start, quat end, float t)
        {
            float eased = Apply(type, t);
            return Mathf.Slerp(start, end, eased);
        }

        public static Color Apply(EasingType type, Color start, Color end, float t)
        {
            float eased = Apply(type, t);
            return new Color(
                start.R + (end.R - start.R) * eased,
                start.G + (end.G - start.G) * eased,
                start.B + (end.B - start.B) * eased,
                start.A + (end.A - start.A) * eased
            );
        }

        public static float Apply(EasingType type, float t)
        {
            t = Mathf.Clamp(t, 0f, 1f);

            return type switch
            {
                EasingType.Linear => Linear(t),
                EasingType.EaseInQuad => EaseInQuad(t),
                EasingType.EaseOutQuad => EaseOutQuad(t),
                EasingType.EaseInOutQuad => EaseInOutQuad(t),
                EasingType.EaseInCubic => EaseInCubic(t),
                EasingType.EaseOutCubic => EaseOutCubic(t),
                EasingType.EaseInOutCubic => EaseInOutCubic(t),
                EasingType.EaseInQuart => EaseInQuart(t),
                EasingType.EaseOutQuart => EaseOutQuart(t),
                EasingType.EaseInOutQuart => EaseInOutQuart(t),
                EasingType.EaseInQuint => EaseInQuint(t),
                EasingType.EaseOutQuint => EaseOutQuint(t),
                EasingType.EaseInOutQuint => EaseInOutQuint(t),
                EasingType.EaseInSine => EaseInSine(t),
                EasingType.EaseOutSine => EaseOutSine(t),
                EasingType.EaseInOutSine => EaseInOutSine(t),
                EasingType.EaseInExpo => EaseInExpo(t),
                EasingType.EaseOutExpo => EaseOutExpo(t),
                EasingType.EaseInOutExpo => EaseInOutExpo(t),
                EasingType.EaseInCirc => EaseInCirc(t),
                EasingType.EaseOutCirc => EaseOutCirc(t),
                EasingType.EaseInOutCirc => EaseInOutCirc(t),
                EasingType.EaseInElastic => EaseInElastic(t),
                EasingType.EaseOutElastic => EaseOutElastic(t),
                EasingType.EaseInOutElastic => EaseInOutElastic(t),
                EasingType.EaseInBack => EaseInBack(t),
                EasingType.EaseOutBack => EaseOutBack(t),
                EasingType.EaseInOutBack => EaseInOutBack(t),
                EasingType.EaseInBounce => EaseInBounce(t),
                EasingType.EaseOutBounce => EaseOutBounce(t),
                EasingType.EaseInOutBounce => EaseInOutBounce(t),
                EasingType.EaseOutOvershoot => EaseOutOvershoot(t),
                EasingType.EaseSpike => EaseSpike(t),
                EasingType.EaseFlash => EaseFlash(t),
                EasingType.EaseSmoothStep => EaseSmoothStep(t),
                EasingType.EaseSmootherStep => EaseSmootherStep(t),
                EasingType.EasePunch => EasePunch(t),
                EasingType.EaseSpring => EaseSpring(t),
                _ => t
            };
        }
    }
}
