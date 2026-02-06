using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace GlmNet
{
    /// <summary>
    /// Represents a 4x4 matrix.
    /// </summary>
    public struct mat4
    {
        public vec4 c0, c1, c2, c3;

        #region Construction

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public mat4(float scale)
        {
            c0 = new vec4(scale, 0, 0, 0);
            c1 = new vec4(0, scale, 0, 0);
            c2 = new vec4(0, 0, scale, 0);
            c3 = new vec4(0, 0, 0, scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public mat4(vec4 a, vec4 b, vec4 c, vec4 d)
        {
            c0 = a; c1 = b; c2 = c; c3 = d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static mat4 identity()
        {
            return new mat4(
                new vec4(1, 0, 0, 0),
                new vec4(0, 1, 0, 0),
                new vec4(0, 0, 1, 0),
                new vec4(0, 0, 0, 1)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static mat4 zero()
        {
            return new mat4(
                new vec4(0, 0, 0, 0),
                new vec4(0, 0, 0, 0),
                new vec4(0, 0, 0, 0),
                new vec4(0, 0, 0, 0)
            );
        }

        #endregion

        #region Index Access

        public vec4 this[int column]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => column switch
            {
                0 => c0,
                1 => c1,
                2 => c2,
                3 => c3,
                _ => throw new IndexOutOfRangeException()
            };
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (column)
                {
                    case 0: c0 = value; break;
                    case 1: c1 = value; break;
                    case 2: c2 = value; break;
                    case 3: c3 = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public float this[int column, int row]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return column switch
                {
                    0 => c0[row],
                    1 => c1[row],
                    2 => c2[row],
                    3 => c3[row],
                    _ => throw new IndexOutOfRangeException()
                };
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch (column)
                {
                    case 0: var v0 = c0; v0[row] = value; c0 = v0; break;
                    case 1: var v1 = c1; v1[row] = value; c1 = v1; break;
                    case 2: var v2 = c2; v2[row] = value; c2 = v2; break;
                    case 3: var v3 = c3; v3[row] = value; c3 = v3; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        #endregion

        #region Conversion

        private static readonly float[] _cached = new float[16];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float[] to_array()
        {
            _cached[0] = c0.x; _cached[1] = c0.y; _cached[2] = c0.z; _cached[3] = c0.w;
            _cached[4] = c1.x; _cached[5] = c1.y; _cached[6] = c1.z; _cached[7] = c1.w;
            _cached[8] = c2.x; _cached[9] = c2.y; _cached[10] = c2.z; _cached[11] = c2.w;
            _cached[12] = c3.x; _cached[13] = c3.y; _cached[14] = c3.z; _cached[15] = c3.w;
            return _cached;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void to_array(Span<float> destination)
        {
            if (destination.Length < 16)
                throw new ArgumentException("Destination span must be at least 16 elements long.");

            destination[0] = c0.x; destination[1] = c0.y; destination[2] = c0.z; destination[3] = c0.w;
            destination[4] = c1.x; destination[5] = c1.y; destination[6] = c1.z; destination[7] = c1.w;
            destination[8] = c2.x; destination[9] = c2.y; destination[10] = c2.z; destination[11] = c2.w;
            destination[12] = c3.x; destination[13] = c3.y; destination[14] = c3.z; destination[15] = c3.w;
        }

        #endregion

        #region Multiplication (SIMD-accelerated, column-major)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector4 ToSimd(vec4 v) => new Vector4(v.x, v.y, v.z, v.w);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static vec4 FromSimd(Vector4 v) => new vec4(v.X, v.Y, v.Z, v.W);

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static vec4 operator *(mat4 lhs, vec4 rhs)
        {
            var x = lhs.c0.x * rhs.x + lhs.c1.x * rhs.y + lhs.c2.x * rhs.z + lhs.c3.x * rhs.w;
            var y = lhs.c0.y * rhs.x + lhs.c1.y * rhs.y + lhs.c2.y * rhs.z + lhs.c3.y * rhs.w;
            var z = lhs.c0.z * rhs.x + lhs.c1.z * rhs.y + lhs.c2.z * rhs.z + lhs.c3.z * rhs.w;
            var w = lhs.c0.w * rhs.x + lhs.c1.w * rhs.y + lhs.c2.w * rhs.z + lhs.c3.w * rhs.w;
            return new vec4(x, y, z, w);
        }

        /// <summary>
        /// Multiplies two matrices (SIMD accelerated internally).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static mat4 operator *(mat4 lhs, mat4 rhs)
        {
            // Use SIMD for each column of rhs
            var rhs0 = ToSimd(rhs.c0);
            var rhs1 = ToSimd(rhs.c1);
            var rhs2 = ToSimd(rhs.c2);
            var rhs3 = ToSimd(rhs.c3);

            return new mat4(
                FromSimd(ToSimd(lhs.c0) * rhs0.X + ToSimd(lhs.c1) * rhs0.Y + ToSimd(lhs.c2) * rhs0.Z + ToSimd(lhs.c3) * rhs0.W),
                FromSimd(ToSimd(lhs.c0) * rhs1.X + ToSimd(lhs.c1) * rhs1.Y + ToSimd(lhs.c2) * rhs1.Z + ToSimd(lhs.c3) * rhs1.W),
                FromSimd(ToSimd(lhs.c0) * rhs2.X + ToSimd(lhs.c1) * rhs2.Y + ToSimd(lhs.c2) * rhs2.Z + ToSimd(lhs.c3) * rhs2.W),
                FromSimd(ToSimd(lhs.c0) * rhs3.X + ToSimd(lhs.c1) * rhs3.Y + ToSimd(lhs.c2) * rhs3.Z + ToSimd(lhs.c3) * rhs3.W)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static mat4 operator *(mat4 lhs, float s)
        {
            return new mat4(lhs.c0 * s, lhs.c1 * s, lhs.c2 * s, lhs.c3 * s);
        }

        #endregion

        #region Equality / ToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is mat4 m)
                return c0 == m.c0 && c1 == m.c1 && c2 == m.c2 && c3 == m.c3;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return c0.GetHashCode() ^ c1.GetHashCode() ^ c2.GetHashCode() ^ c3.GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(mat4 a, mat4 b) => a.Equals(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(mat4 a, mat4 b) => !a.Equals(b);

        public override string ToString()
        {
            return $"[{c0}, {c1}, {c2}, {c3}]";
        }

        #endregion
    }
}
