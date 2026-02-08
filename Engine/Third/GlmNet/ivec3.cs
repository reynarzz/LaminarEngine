using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GlmNet
{
    /// <summary>
    /// Represents a three dimensional integer vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ivec3
    {
        public int x;
        public int y;
        public int z;

        [JsonIgnore] public ivec2 xy => new ivec2(x, y);
        [JsonIgnore] public static ivec3 One { get; } = new ivec3(1, 1, 1);
        [JsonIgnore] public static ivec3 Zero { get; } = new ivec3(0, 0, 0);
        [JsonIgnore] public static ivec3 Up { get; } = new ivec3(0, 1, 0);
        [JsonIgnore] public static ivec3 Right { get; } = new ivec3(1, 0, 0);
        [JsonIgnore] public static ivec3 Forward { get; } = new ivec3(0, 0, 1);

        [JsonIgnore]
        public int this[int index]
        {
            get
            {
                if (index == 0) return x;
                else if (index == 1) return y;
                else if (index == 2) return z;
                else throw new Exception("Out of range.");
            }
            set
            {
                if (index == 0) x = value;
                else if (index == 1) y = value;
                else if (index == 2) z = value;
                else throw new Exception("Out of range.");
            }
        }

        public ivec3(ivec2 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = 0;
        }

        public ivec3(int s)
        {
            x = y = z = s;
        }

        public ivec3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public ivec3(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public ivec3(ivec3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public ivec3(ivec2 xy, int z)
        {
            this.x = xy.x;
            this.y = xy.y;
            this.z = z;
        }

        public static ivec3 operator +(ivec3 lhs, ivec3 rhs)
        {
            return new ivec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static ivec3 operator +(ivec3 lhs, int rhs)
        {
            return new ivec3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
        }

        public static ivec3 operator -(ivec3 lhs, ivec3 rhs)
        {
            return new ivec3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static ivec3 operator -(ivec3 vec)
        {
            return new ivec3(-vec.x, -vec.y, -vec.z);
        }

        public static ivec3 operator *(ivec3 lhs, int rhs)
        {
            return new ivec3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }

        public static ivec3 operator *(int lhs, ivec3 rhs)
        {
            return new ivec3(rhs.x * lhs, rhs.y * lhs, rhs.z * lhs);
        }

        public static ivec3 operator *(ivec3 lhs, ivec3 rhs)
        {
            return new ivec3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }

        public int[] to_array()
        {
            return new[] { x, y, z };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void to_array(Span<int> arr)
        {
            arr[0] = x;
            arr[1] = y;
            arr[2] = z;
        }

        #region Comparison
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ivec3))
            {
                var vec = (ivec3)obj;
                return this.x == vec.x && this.y == vec.y && this.z == vec.z;
            }
            return false;
        }

        public static bool operator ==(ivec3 v1, ivec3 v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(ivec3 v1, ivec3 v2)
        {
            return !v1.Equals(v2);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }
        #endregion

        #region ToString support
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }
        #endregion
    }
}
