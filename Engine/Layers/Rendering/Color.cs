using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public struct Color
    {
        public float R, G, B, A;
        public readonly static Color Transparent = new Color(1, 1, 1, 0);
        public readonly static Color Red = new Color(1, 0, 0, 1);
        public readonly static Color Green = new Color(0, 1, 0, 1);
        public readonly static Color Blue = new Color(0, 0, 1, 1);
        public readonly static Color White = new Color(1, 1, 1, 1);
        public readonly static Color Black = new Color(0, 0, 0, 1);
        public readonly static Color Yellow = new Color(1f, 1f, 0f, 1f);
        public readonly static Color Cyan = new Color(0f, 1f, 1f, 1f);
        public readonly static Color Magenta = new Color(1f, 0f, 1f, 1f);
        public readonly static Color Gray = new Color(0.5f, 0.5f, 0.5f, 1f);
        public readonly static Color LightGray = new Color(0.83f, 0.83f, 0.83f, 1f);
        public readonly static Color DarkGray = new Color(0.33f, 0.33f, 0.33f, 1f);
        public readonly static Color Orange = new Color(1f, 0.65f, 0f, 1f);
        public readonly static Color Purple = new Color(0.5f, 0f, 0.5f, 1f);
        public readonly static Color Brown = new Color(0.6f, 0.3f, 0f, 1f);
        public readonly static Color Pink = new Color(1f, 0.75f, 0.8f, 1f);
        public readonly static Color LightBlue = new Color(0.68f, 0.85f, 0.9f, 1f);
        public readonly static Color DarkBlue = new Color(0f, 0f, 0.55f, 1f);
        public readonly static Color LightGreen = new Color(0.56f, 0.93f, 0.56f, 1f);
        public readonly static Color DarkGreen = new Color(0f, 0.39f, 0f, 1f);
        public readonly static Color Teal = new Color(0f, 0.5f, 0.5f, 1f);
        public readonly static Color Navy = new Color(0f, 0f, 0.5f, 1f);
        public readonly static Color Gold = new Color(1f, 0.84f, 0f, 1f);
        public readonly static Color Silver = new Color(0.75f, 0.75f, 0.75f, 1f);
        public readonly static Color Olive = new Color(0.5f, 0.5f, 0f, 1f);
        public readonly static Color Maroon = new Color(0.5f, 0f, 0f, 1f);
        public readonly static Color Violet = new Color(0.93f, 0.51f, 0.93f, 1f);
        public readonly static Color Coral = new Color(1f, 0.5f, 0.31f, 1f);
        public readonly static Color Salmon = new Color(0.98f, 0.5f, 0.45f, 1f);
        public readonly static Color Indigo = new Color(0.29f, 0f, 0.51f, 1f);

        public Color(float r, float g, float b, float a = 1f)
        {
            R = Mathf.Clamp01(r);
            G = Mathf.Clamp01(g);
            B = Mathf.Clamp01(b);
            A = Mathf.Clamp01(a);
        }

        public static implicit operator ColorPacketRGBA(Color c)
        {
            return (ColorPacketRGBA)(Color32)c;
        }

        public static implicit operator uint(Color c)
        {
            return (ColorPacketRGBA)(Color32)c;
        }

        public static implicit operator Color(uint packet)
        {
            return new ColorPacketRGBA(packet);
        }

        public static implicit operator vec4(Color color)
        {
            return new vec4(color.R, color.G, color.B, color.A);
        }
        public static implicit operator vec3(Color color)
        {
            return new vec3(color.R, color.G, color.B);
        }

        public static Color Lerp(Color a, Color b, float t)
        {
            t = Mathf.Clamp01(t);
            return new Color(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t,
                a.A + (b.A - a.A) * t
            );
        }
        public static Color MoveTowards(Color current, Color target, float maxStep)
        {
            float r = MoveChannel(current.R, target.R, maxStep);
            float g = MoveChannel(current.G, target.G, maxStep);
            float b = MoveChannel(current.B, target.B, maxStep);
            float a = MoveChannel(current.A, target.A, maxStep);

            return new Color(r, g, b, a);
        }

        private static float MoveChannel(float from, float to, float maxStep)
        {
            float delta = to - from;

            if (Math.Abs(delta) <= maxStep)
                return to;

            return from + Math.Sign(delta) * maxStep;
        }
        public static Color Hermite(Color startValue, Color startTangent, Color endTangent, Color endValue, float t)
        {
            t = Mathf.Clamp01(t);
            float t2 = t * t;
            float t3 = t2 * t;

            float h00 = 2f * t3 - 3f * t2 + 1f;
            float h10 = t3 - 2f * t2 + t;
            float h01 = -2f * t3 + 3f * t2;
            float h11 = t3 - t2;

            return new Color(
                h00 * startValue.R + h10 * startTangent.R + h01 * endValue.R + h11 * endTangent.R,
                h00 * startValue.G + h10 * startTangent.G + h01 * endValue.G + h11 * endTangent.G,
                h00 * startValue.B + h10 * startTangent.B + h01 * endValue.B + h11 * endTangent.B,
                h00 * startValue.A + h10 * startTangent.A + h01 * endValue.A + h11 * endTangent.A
            );
        }

        public static Color RandomRGB()
        {
            return new Color(Random.Shared.NextSingle(), Random.Shared.NextSingle(), Random.Shared.NextSingle(), 1.0f);
        }

        public static Color operator *(Color col, float scale)
        {
            return new Color(col.R * scale, col.G * scale, col.B * scale, col.A * scale);
        }

        public uint ToARGB_U32()
        {
            var color = (Color32)this;
            return ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
        }

        public Vector4 ToVector4()
        {
            return new Vector4(R, G, B, A);
        }
    }

    public struct Color32
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color32(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public static Color32 RGB(byte value)
        {
            return new Color32(value, value, value, 255);
        }

        public static implicit operator Color(Color32 c32)
        {
            return new Color(
                c32.R / 255.0f,
                c32.G / 255.0f,
                c32.B / 255.0f,
                c32.A / 255.0f);
        }

        public static implicit operator ColorPacketRGBA(Color32 c32)
        {
            uint packed =
                ((uint)c32.R << 24) |
                ((uint)c32.G << 16) |
                ((uint)c32.B << 8) |
                c32.A;
            return new ColorPacketRGBA(packed);
        }

        public static implicit operator Color32(ColorPacketRGBA packet)
        {
            uint v = packet.Value;
            byte r = (byte)(v >> 24);
            byte g = (byte)(v >> 16);
            byte b = (byte)(v >> 8);
            byte a = (byte)v;
            return new Color32(r, g, b, a);
        }

        public static implicit operator Color32(Color color)
        {
            return new Color32((byte)(color.R * 255.0f), (byte)(color.G * 255.0f), (byte)(color.B * 255.0f), (byte)(color.A * 255.0f));
        }
    }

    public struct ColorPacketRGBA
    {
        public uint Value;

        public ColorPacketRGBA(uint value)
        {
            Value = value;
        }

        public static implicit operator Color(ColorPacketRGBA packet)
        {
            return (Color)(Color32)packet;
        }

        public static implicit operator uint(ColorPacketRGBA packet)
        {
            return packet.Value;
        }

        public static implicit operator ColorPacketRGBA(uint packet)
        {
            return new ColorPacketRGBA(packet);
        }
    }
}
