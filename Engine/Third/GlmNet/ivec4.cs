using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GlmNet
{
    /// <summary>
    /// Represents a four dimensional integer vector.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ivec4
    {
        public int x;
        public int y;
        public int z;
        public int w;

        [JsonIgnore] public ivec2 xy => new ivec2(x, y);
        [JsonIgnore] public ivec3 xyz => new ivec3(x, y, z);

        [JsonIgnore] public static ivec4 One { get; } = new ivec4(1, 1, 1, 1);
        [JsonIgnore] public static ivec4 Zero { get; } = new ivec4(0, 0, 0, 0);

        [JsonIgnore]
        public int this[int index]
        {
            get
            {
                if (index == 0) return x;
                else if (index == 1) return y;
                else if (index == 2) return z;
                else if (index == 3) return w;
                else throw new Exception("Out of range.");
            }
            set
            {
                if (index == 0) x = value;
                else if (index == 1) y = value;
                else if (index == 2) z = value;
                else if (index == 3) w = value;
                else throw new Exception("Out of range.");
            }
        }

        public ivec4(int s)
        {
            x = y = z = w = s;
        }

        public ivec4(int x, int y, int z, int w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public ivec4(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 0;
        }

        public ivec4(ivec2 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = 0;
            this.w = 0;
        }

        public ivec4(ivec3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = 0;
        }

        public ivec4(ivec4 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }

        public ivec4(ivec3 xyz, int w)
        {
            this.x = xyz.x;
            this.y = xyz.y;
            this.z = xyz.z;
            this.w = w;
        }

        public static ivec4 operator +(ivec4 lhs, ivec4 rhs)
        {
            return new ivec4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
        }

        public static ivec4 operator +(ivec4 lhs, int rhs)
        {
            return new ivec4(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs, lhs.w + rhs);
        }

        public static ivec4 operator -(ivec4 lhs, ivec4 rhs)
        {
            return new ivec4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
        }

        public static ivec4 operator -(ivec4 vec)
        {
            return new ivec4(-vec.x, -vec.y, -vec.z, -vec.w);
        }

        public static ivec4 operator *(ivec4 lhs, int rhs)
        {
            return new ivec4(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
        }

        public static ivec4 operator *(int lhs, ivec4 rhs)
        {
            return new ivec4(rhs.x * lhs, rhs.y * lhs, rhs.z * lhs, rhs.w * lhs);
        }

        public static ivec4 operator *(ivec4 lhs, ivec4 rhs)
        {
            return new ivec4(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z, lhs.w * rhs.w);
        }

        public int[] to_array()
        {
            return new[] { x, y, z, w };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void to_array(Span<int> arr)
        {
            arr[0] = x;
            arr[1] = y;
            arr[2] = z;
            arr[3] = w;
        }

        #region Comparison
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(ivec4))
            {
                var vec = (ivec4)obj;
                return this.x == vec.x && this.y == vec.y && this.z == vec.z && this.w == vec.w;
            }
            return false;
        }

        public static bool operator ==(ivec4 v1, ivec4 v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(ivec4 v1, ivec4 v2)
        {
            return !v1.Equals(v2);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }
        #endregion

        #region ToString support
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
        }
        #endregion
    }
}
