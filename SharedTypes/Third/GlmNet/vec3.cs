using System;
using System.Runtime.CompilerServices;

namespace GlmNet
{
    /// <summary>
    /// Represents a three dimensional vector.
    /// </summary>
    public struct vec3
    {
        public float x;
        public float y;
        public float z;

        public static vec3 One { get; } = new vec3(1, 1, 1);
        public static vec3 Zero { get; } = new vec3(0, 0, 0);
        public static vec3 Up { get; } = new vec3(0, 1, 0);
        public static vec3 Right { get; } = new vec3(1, 0, 0);
        public static vec3 Forward { get; } = new vec3(0, 0, 1);
        public static vec3 Half { get; } = new vec3(0.5f, 0.5f, 0.5f);

        public float this[int index]
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

        public vec3(vec2 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = 0;
        }

        public vec3(float s)
        {
            x = y = z = s;
        }

        public vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public vec3(vec3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public vec3(vec4 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public vec3(vec2 xy, float z)
        {
            this.x = xy.x;
            this.y = xy.y;
            this.z = z;
        }

        public static vec3 operator +(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static vec3 operator +(vec3 lhs, float rhs)
        {
            return new vec3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
        }

        public static vec3 operator -(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static vec3 operator -(vec3 vec)
        {
            return new vec3(-vec.x, -vec.y, -vec.z);
        }

        //public static vec3 operator -(vec3 lhs, float rhs)
        //{
        //    return new vec3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
        //}

        public static vec3 operator *(vec3 self, float s)
        {
            return new vec3(self.x * s, self.y * s, self.z * s);
        }
        public static vec3 operator *(float lhs, vec3 rhs)
        {
            return new vec3(rhs.x * lhs, rhs.y * lhs, rhs.z * lhs);
        }

        public static vec3 operator /(vec3 lhs, float rhs)
        {
            return new vec3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
        }

        public static vec3 operator *(vec3 lhs, vec3 rhs)
        {
            return new vec3(rhs.x * lhs.x, rhs.y * lhs.y, rhs.z * lhs.z);
        }

        public static implicit operator vec2(vec3 v)
        {
            return new vec2(v.x, v.y);
        }

        public static implicit operator vec3(vec2 v)
        {
            return new vec3(v.x, v.y, 0);
        }
        public vec3 Normalized
        {
            get
            {
                float len = Magnitude;
                return len > 0f ? this / len : Zero;
            }
        }

        public float Magnitude
        {
            get
            {
                return MathF.Sqrt(x * x + y * y + z * z);
            }
        }

        public float SqrMagnitude
        {
            get
            {
                return x * x + y * y + z * z;
            }
        }


        public float[] to_array()
        {
            return new[] { x, y, z };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void to_array(Span<float> arr)
        {
            arr[0] = x;
            arr[1] = y;
            arr[2] = z;
        }

        #region Comparision

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// The Difference is detected by the different values
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(vec3))
            {
                var vec = (vec3)obj;
                if (this.x == vec.x && this.y == vec.y && this.z == vec.z)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="v1">The first Vector.</param>
        /// <param name="v2">The second Vector.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(vec3 v1, vec3 v2)
        {
            return v1.Equals(v2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="v1">The first Vector.</param>
        /// <param name="v2">The second Vector.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(vec3 v1, vec3 v2)
        {
            return !v1.Equals(v2);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode();
        }

        #endregion

        #region ToString support

        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }

        public float length()
        {
            return MathF.Sqrt(glm.dot(this, this));
        }

        #endregion
    }
}
