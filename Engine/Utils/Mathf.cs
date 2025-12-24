using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct Mathf
    {
        public const float E = 2.7182818284590452354f;

        public const float PI = 3.14159265358979323846f;

        public const float Tau = 6.283185307179586476925f;
        public static float Clamp(float value, float min, float max)
           => value < min ? min : (value > max ? max : value);

        public static int Clamp(int value, int min, int max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        public static float Clamp01(float v) => Clamp(v, 0f, 1f);

        public static float Lerp(float a, float b, float t)
            => a + (b - a) * Clamp01(t);

        public static float Dot(vec2 a, vec2 b)
            => a.x * b.x + a.y * b.y;

        public static float Dot(vec3 a, vec3 b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        public static float Dot(quat a, quat b)
            => a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;

        public static quat Mul(quat q, float s)
            => new quat(q.x * s, q.y * s, q.z * s, q.w * s);

        public static quat Mul(float s, quat q)
            => new quat(q.x * s, q.y * s, q.z * s, q.w * s);

        public static quat Lerp(quat a, quat b, float t)
        {
            // (a*(1-t) + b*t).Normalized
            quat result = Add(Mul(a, 1 - t), Mul(b, t));

            result.Normalize();

            return result;
        }

        public static float PingPong(float t, float length = 1f)
        {
            t = t % (2f * length);
            return length - Math.Abs(t - length);
        }
        public static float Wrap(float t, float length)
        {
            return t - MathF.Floor(t / length) * length;
        }
        public static float SmoothLoop(float t, float length)
        {
            // t normalized 
            float x = (t / length) * MathF.PI * 2f;

            return (MathF.Sin(x) * 0.5f + 0.5f) * length;
        }

        public static quat Slerp(quat a, quat b, float t)
        {
            float dot = Dot(a, b);

            // If dot < 0, invert one to take shortest path
            if (dot < 0.0f)
            {
                b = new quat(-b.x, -b.y, -b.z, -b.w);
                dot = -dot;
            }

            const float DOT_THRESHOLD = 0.9995f;
            if (dot > DOT_THRESHOLD)
            {
                // Very close - do linear and normalize
                quat result = Add(a, Mul(Sub(b, a), t));
                result.Normalize();
                return result;
            }

            float theta_0 = (float)System.Math.Acos(dot); // angle between input quats
            float theta = theta_0 * t;
            quat c = Sub(b, Mul(a, dot));
            c.Normalize(); // orthonormal basis

            float cosTheta = (float)System.Math.Cos(theta);
            float sinTheta = (float)System.Math.Sin(theta);

            return Add(Mul(a, cosTheta), Mul(c, sinTheta));
        }

        private static quat Add(quat a, quat b)
            => new quat(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        private static quat Sub(quat a, quat b)
            => new quat(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        public static vec2 Lerp(vec2 a, vec2 b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }


        public static vec3 Lerp(vec3 a, vec3 b, float t)
            => a + (b - a) * Clamp01(t);

        public static vec3 SmoothLerp(vec3 a, vec3 b, float t)
        {
            t = t * t * (3 - 2 * t);
            return Lerp(a, b, t);
        }

        public static vec3 Round(vec3 v)
        {
            return new vec3(MathF.Round(v.x), MathF.Round(v.y), MathF.Round(v.z));
        }

        public static vec3 Floor(vec3 v)
        {
            return new vec3(MathF.Floor(v.x), MathF.Floor(v.y), MathF.Floor(v.z));
        }
        public static float Floor(float v)
        {
            return MathF.Floor(v);
        }

        public static int Floor(int v)
        {
            return Floor(v);
        }
        public static int FloorToInt(float v)
        {
            return (int)Floor(v);
        }
        public static int RoundToInt(float v)
        {
            return (int)MathF.Round(v);
        }
        public static int CeilToInt(float v)
        {
            return (int)MathF.Ceiling(v);
        }

        public static int RoundToInt(double v)
        {
            return (int)Math.Round(v);
        }
        public static vec3 Ceil(vec3 v)
        {
            return new vec3(MathF.Ceiling(v.x), MathF.Ceiling(v.y), MathF.Ceiling(v.z));
        }

        public static vec2 Round(vec2 v)
        {
            return new vec2(MathF.Round(v.x), MathF.Round(v.y));
        }

        public static vec2 Floor(vec2 v)
        {
            return new vec2(MathF.Floor(v.x), MathF.Floor(v.y));
        }

        public static vec2 Ceil(vec2 v)
        {
            return new vec2(MathF.Ceiling(v.x), MathF.Ceiling(v.y));
        }

        public static vec3 SmoothDamp(vec3 current,
                                      vec3 target,
                                      ref vec3 currentVelocity,
                                      float smoothTime,
                                      float maxSpeed = float.PositiveInfinity,
                                      float deltaTime = -1f)
        {
            // if deltaTime not provided, use Time.DeltaTime
            if (deltaTime < 0f)
                deltaTime = Time.DeltaTime;

            // Prevent tiny smoothTime (avoid divide by zero)
            smoothTime = Math.Max(0.0001f, smoothTime);

            float omega = 2f / smoothTime;

            float x = omega * deltaTime;
            float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

            vec3 change = current - target;
            vec3 originalTo = target;

            // Clamp maximum speed
            float maxChange = maxSpeed * smoothTime;
            float maxChangeSq = maxChange * maxChange;

            // Inline squared magnitude
            float sqrmag = change.x * change.x + change.y * change.y + change.z * change.z;
            if (sqrmag > maxChangeSq)
            {
                float mag = MathF.Sqrt(sqrmag);
                change *= maxChange / mag;
            }

            target = current - change;

            vec3 temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * exp;

            vec3 output = target + (change + temp) * exp;

            vec3 origToCurrent = originalTo - current;
            vec3 outToOrig = output - originalTo;

            if (Dot(origToCurrent, outToOrig) > 0f)
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        public static vec3 SineLerp(vec3 a, vec3 b, float t)
        {
            t = 0.5f - 0.5f * glm.cos(t * (float)Math.PI);
            return Lerp(a, b, t);
        }
        public static vec2 MoveTowards(vec2 current, vec2 target, float maxDistanceDelta)
        {
            vec2 toVector = target - current;
            float dist = MathF.Sqrt(toVector.x * toVector.x + toVector.y * toVector.y);

            if (dist <= maxDistanceDelta || dist == 0f)
                return target;

            return current + toVector / dist * maxDistanceDelta;
        }

        public static vec3 MoveTowards(vec3 current, vec3 target, float maxDistanceDelta)
        {
            vec3 toVector = target - current;
            float dist = MathF.Sqrt(toVector.x * toVector.x + toVector.y * toVector.y + toVector.z * toVector.z);

            if (dist <= maxDistanceDelta || dist == 0f)
                return target;

            return current + toVector / dist * maxDistanceDelta;
        }

        public static float Distance(vec3 a, vec3 b)
        {
            vec3 c = a - b;
            return MathF.Sqrt(c.x * c.x + c.y * c.y + c.z * c.z);
        }

        public static float Distance(vec2 a, vec2 b)
        {
            vec2 c = a - b;
            return MathF.Sqrt(c.x * c.x + c.y * c.y);
        }

        public static vec2 Clamp(vec2 v, vec2 min, vec2 max)
            => new vec2(
                Clamp(v.x, min.x, max.x),
                Clamp(v.y, min.y, max.y));

        public static vec3 Clamp(vec3 v, vec3 min, vec3 max)
        {
            return new vec3(Clamp(v.x, min.x, max.x),
                 Clamp(v.y, min.y, max.y),
                 Clamp(v.z, min.z, max.z));
        }


        
        public static mat4 QuatToMat4(quat q)
        {
            float x = q.x, y = q.y, z = q.z, w = q.w;

            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;
            float wx = w * x;
            float wy = w * y;
            float wz = w * z;

            mat4 result = mat4.identity();

            // column 0
            result[0, 0] = 1 - 2 * (yy + zz);
            result[0, 1] = 2 * (xy + wz);
            result[0, 2] = 2 * (xz - wy);
            result[0, 3] = 0f;

            // column 1
            result[1, 0] = 2 * (xy - wz);
            result[1, 1] = 1 - 2 * (xx + zz);
            result[1, 2] = 2 * (yz + wx);
            result[1, 3] = 0f;

            // column 2
            result[2, 0] = 2 * (xz + wy);
            result[2, 1] = 2 * (yz - wx);
            result[2, 2] = 1 - 2 * (xx + yy);
            result[2, 3] = 0f;

            // column 3 (translation)
            result[3, 0] = 0f;
            result[3, 1] = 0f;
            result[3, 2] = 0f;
            result[3, 3] = 1f;

            return result;
        }

        public static float Hermite(float startValue, float startTangent, float endTangent, float endValue, float t)
        {
            t = Clamp01(t);
            float t2 = t * t;
            float t3 = t2 * t;

            float h00 = 2f * t3 - 3f * t2 + 1f;
            float h10 = t3 - 2f * t2 + t;
            float h01 = -2f * t3 + 3f * t2;
            float h11 = t3 - t2;

            return h00 * startValue + h10 * startTangent + h01 * endValue + h11 * endTangent;
        }

        public static vec2 Hermite(vec2 startValue, vec2 startTangent, vec2 endTangent, vec2 endValue, float t)
        {
            t = Clamp01(t);
            float t2 = t * t;
            float t3 = t2 * t;

            float h00 = 2f * t3 - 3f * t2 + 1f;
            float h10 = t3 - 2f * t2 + t;
            float h01 = -2f * t3 + 3f * t2;
            float h11 = t3 - t2;

            return h00 * startValue + h10 * startTangent + h01 * endValue + h11 * endTangent;
        }

        public static vec3 Hermite(vec3 startValue, vec3 startTangent, vec3 endTangent, vec3 endValue, float t)
        {
            t = Clamp01(t);
            float t2 = t * t;
            float t3 = t2 * t;

            float h00 = 2f * t3 - 3f * t2 + 1f;
            float h10 = t3 - 2f * t2 + t;
            float h01 = -2f * t3 + 3f * t2;
            float h11 = t3 - t2;

            return h00 * startValue + h10 * startTangent + h01 * endValue + h11 * endTangent;
        }

        public static vec4 Hermite(vec4 startValue, vec4 startTangent, vec4 endTangent, vec4 endValue, float t)
        {
            t = Clamp01(t);
            float t2 = t * t;
            float t3 = t2 * t;

            float h00 = 2f * t3 - 3f * t2 + 1f;
            float h10 = t3 - 2f * t2 + t;
            float h01 = -2f * t3 + 3f * t2;
            float h11 = t3 - t2;

            return h00 * startValue + h10 * startTangent + h01 * endValue + h11 * endTangent;
        }

        public static quat Hermite(quat startValue, quat startTangent, quat endTangent, quat endValue, float t)
        {
            t = Clamp01(t);
            float t2 = t * t;
            float t3 = t2 * t;

            float h00 = 2f * t3 - 3f * t2 + 1f;
            float h10 = t3 - 2f * t2 + t;
            float h01 = -2f * t3 + 3f * t2;
            float h11 = t3 - t2;

            quat result = h00 * startValue + h10 * startTangent + h01 * endValue + h11 * endTangent;

            return Normalize(result);
        }

        public static quat Normalize(quat value)
        {
            float length = MathF.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z + value.w * value.w);
            return length > 0f ? value / length : value;
        }

        public static vec3 Min(vec3 a, vec3 b)
        {
            return new vec3(MathF.Min(a.x, b.x), MathF.Min(a.y, b.y), MathF.Min(a.z, b.z));
        }

        public static vec3 Max(vec3 a, vec3 b)
        {
            return new vec3(MathF.Max(a.x, b.x), MathF.Max(a.y, b.y), MathF.Max(a.z, b.z));
        }

        public static float SmoothNoise1D(float x)
        {
            int i = (int)MathF.Floor(x);
            float f = x - i;

            float a = Hash(i);
            float b = Hash(i + 1);

            // Cosine interpolation
            float ft = f * MathF.PI;
            float s = (1f - MathF.Cos(ft)) * 0.5f;

            return a * (1f - s) + b * s; // returns [-1,1]
        }

        // Hash to deterministic random [-1,1]
        private static float Hash(int x)
        {
            unchecked
            {
                x = (x << 13) ^ x;
                int h = (x * (x * x * 15731 + 789221) + 1376312589);
                return 1f - ((h & 0x7fffffff) / 1073741824f);
            }
        }

        public static vec2 Noise2(float t)
        {
            float x = SmoothNoise1D(t + 0f);
            float y = SmoothNoise1D(t + 100f);
            return new vec2(x, y);
        }

        public static vec3 Noise3(float t)
        {
            float x = SmoothNoise1D(t + 0f);
            float y = SmoothNoise1D(t + 2f);
            float z = SmoothNoise1D(t + 3f);
            return new vec3(x, y, z);
        }

        public static vec3 Noise3Fractal(float t, int octaves = 3)
        {
            float amp = 1f;
            float freq = 1f;

            vec3 sum = vec3.Zero;
            float norm = 0f;

            for (int i = 0; i < octaves; i++)
            {
                sum += Noise3(t * freq) * amp;
                norm += amp;

                amp *= 0.5f;
                freq *= 2f;
            }

            return sum / norm; // stays in [-1,1]
        }

    }
}
