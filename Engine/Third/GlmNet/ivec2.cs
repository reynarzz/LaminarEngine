using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GlmNet
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ivec2
    {
        public int x;
        public int y;

        [JsonIgnore] public static ivec2 One { get; } = new ivec2(1, 1);
        [JsonIgnore] public static ivec2 Zero { get; } = new ivec2(0, 0);
        [JsonIgnore] public static ivec2 Right { get; } = new ivec2(1, 0);
        [JsonIgnore] public static ivec2 Up { get; } = new ivec2(0, 1);

        public int this[int index]
        {
            get
            {
                if (index == 0) return x;
                if (index == 1) return y;
                throw new Exception("Out of range.");
            }
            set
            {
                if (index == 0) x = value;
                else if (index == 1) y = value;
                else throw new Exception("Out of range.");
            }
        }

        public ivec2(int s)
        {
            x = y = s;
        }

        public ivec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public ivec2(ivec2 v)
        {
            x = v.x;
            y = v.y;
        }

        public ivec2(vec2 v)
        {
            x = (int)v.x;
            y = (int)v.y;
        }

        public static ivec2 operator +(ivec2 lhs, ivec2 rhs)
        {
            return new ivec2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static ivec2 operator -(ivec2 vec)
        {
            return new ivec2(-vec.x, -vec.y);
        }

        public static ivec2 operator -(ivec2 lhs, ivec2 rhs)
        {
            return new ivec2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public static ivec2 operator *(ivec2 lhs, int rhs)
        {
            return new ivec2(lhs.x * rhs, lhs.y * rhs);
        }

        public static ivec2 operator /(ivec2 lhs, int rhs)
        {
            return new ivec2(lhs.x / rhs, lhs.y / rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj is ivec2 v)
                return x == v.x && y == v.y;

            return false;
        }

        public static bool operator ==(ivec2 v1, ivec2 v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }
        public static bool operator !=(ivec2 v1, ivec2 v2)
        {
            return !(v1 == v2);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{x}, {y}]";
        }
    }
}
