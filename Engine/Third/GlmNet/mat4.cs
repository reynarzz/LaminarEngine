using System;

namespace GlmNet
{
    /// <summary>
    /// Represents a 4x4 matrix.
    /// </summary>
    public struct mat4
    {
        public vec4 c0, c1, c2, c3;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="mat4"/> struct.
        /// This matrix is the identity matrix scaled by <paramref name="scale"/>.
        /// </summary>
        /// <param name="scale">The scale.</param>
        public mat4(float scale)
        {
            c0 = new vec4(scale, 0, 0, 0);
            c1 = new vec4(0, scale, 0, 0);
            c2 = new vec4(0, 0, scale, 0);
            c3 = new vec4(0, 0, 0, scale);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="mat4"/> struct.
        /// The matrix is initialised with the specified columns.
        /// </summary>
        /// <param name="a">First column.</param>
        /// <param name="b">Second column.</param>
        /// <param name="c">Third column.</param>
        /// <param name="d">Fourth column.</param>
        public mat4(vec4 a, vec4 b, vec4 c, vec4 d)
        {
            c0 = a;
            c1 = b;
            c2 = c;
            c3 = d;
        }

        /// <summary>
        /// Creates an identity matrix.
        /// </summary>
        /// <returns>A new identity matrix.</returns>
        public static mat4 identity()
        {
            return new mat4(
                new vec4(1, 0, 0, 0),
                new vec4(0, 1, 0, 0),
                new vec4(0, 0, 1, 0),
                new vec4(0, 0, 0, 1)
            );
        }

        #endregion

        #region Index Access

        /// <summary>
        /// Gets or sets the <see cref="vec4"/> column at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="vec4"/> column.
        /// </value>
        /// <param name="column">The column index.</param>
        /// <returns>The column at index <paramref name="column"/>.</returns>
        public vec4 this[int column]
        {
            get
            {
                return column switch
                {
                    0 => c0,
                    1 => c1,
                    2 => c2,
                    3 => c3,
                    _ => throw new IndexOutOfRangeException()
                };
            }
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

        /// <summary>
        /// Gets or sets the element at <paramref name="column"/> and <paramref name="row"/>.
        /// </summary>
        /// <value>
        /// The element at <paramref name="column"/> and <paramref name="row"/>.
        /// </value>
        /// <param name="column">The column index.</param>
        /// <param name="row">The row index.</param>
        /// <returns>
        /// The element at <paramref name="column"/> and <paramref name="row"/>.
        /// </returns>
        public float this[int column, int row]
        {
            get => this[column][row];
            set
            {
                var col = this[column];
                col[row] = value;
                this[column] = col;
            }
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Returns the matrix as a flat array of elements, column major.
        /// </summary>
        /// <returns></returns>
        public float[] to_array()
        {
            var result = new float[16];
            result[0] = c0.x; result[1] = c0.y; result[2] = c0.z; result[3] = c0.w;
            result[4] = c1.x; result[5] = c1.y; result[6] = c1.z; result[7] = c1.w;
            result[8] = c2.x; result[9] = c2.y; result[10] = c2.z; result[11] = c2.w;
            result[12] = c3.x; result[13] = c3.y; result[14] = c3.z; result[15] = c3.w;
            return result;
        }

        /// <summary>
        /// Writes the matrix into the specified destination span (column major).
        /// </summary>
        /// <param name="destination">The destination span to write into. Must have at least 16 elements.</param>
        public void to_array(Span<float> destination)
        {
            if (destination.Length < 16)
                throw new ArgumentException("Destination span must be at least 16 elements long.");

            destination[0] = c0.x; destination[1] = c0.y; destination[2] = c0.z; destination[3] = c0.w;
            destination[4] = c1.x; destination[5] = c1.y; destination[6] = c1.z; destination[7] = c1.w;
            destination[8] = c2.x; destination[9] = c2.y; destination[10] = c2.z; destination[11] = c2.w;
            destination[12] = c3.x; destination[13] = c3.y; destination[14] = c3.z; destination[15] = c3.w;
        }

        /// <summary>
        /// Returns the <see cref="mat3"/> portion of this matrix.
        /// </summary>
        /// <returns>The <see cref="mat3"/> portion of this matrix.</returns>
        public mat3 to_mat3()
        {
            return new mat3(
                new vec3(c0.x, c0.y, c0.z),
                new vec3(c1.x, c1.y, c1.z),
                new vec3(c2.x, c2.y, c2.z)
            );
        }

        #endregion

        #region Multiplication

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> vector.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS vector.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static vec4 operator *(mat4 lhs, vec4 rhs)
        {
            return new vec4(
                lhs.c0.x * rhs.x + lhs.c1.x * rhs.y + lhs.c2.x * rhs.z + lhs.c3.x * rhs.w,
                lhs.c0.y * rhs.x + lhs.c1.y * rhs.y + lhs.c2.y * rhs.z + lhs.c3.y * rhs.w,
                lhs.c0.z * rhs.x + lhs.c1.z * rhs.y + lhs.c2.z * rhs.z + lhs.c3.z * rhs.w,
                lhs.c0.w * rhs.x + lhs.c1.w * rhs.y + lhs.c2.w * rhs.z + lhs.c3.w * rhs.w
            );
        }

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> matrix.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS matrix.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static mat4 operator *(mat4 lhs, mat4 rhs)
        {
            mat4 result = new mat4();
            result.c0 = lhs * rhs.c0;
            result.c1 = lhs * rhs.c1;
            result.c2 = lhs * rhs.c2;
            result.c3 = lhs * rhs.c3;
            return result;
        }

        public static mat4 operator *(mat4 lhs, float s)
        {
            return new mat4(lhs.c0 * s, lhs.c1 * s, lhs.c2 * s, lhs.c3 * s);
        }

        #endregion

        #region ToString support

        public override string ToString()
        {
            return $"[{c0.x}, {c1.x}, {c2.x}, {c3.x}; " +
                   $"{c0.y}, {c1.y}, {c2.y}, {c3.y}; " +
                   $"{c0.z}, {c1.z}, {c2.z}, {c3.z}; " +
                   $"{c0.w}, {c1.w}, {c2.w}, {c3.w}]";
        }

        #endregion

        #region Comparison

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// The Difference is detected by the different values.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(mat4))
            {
                var mat = (mat4)obj;
                if (mat.c0 == this.c0 && mat.c1 == this.c1 && mat.c2 == this.c2 && mat.c3 == this.c3)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="m1">The first Matrix.</param>
        /// <param name="m2">The second Matrix.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(mat4 m1, mat4 m2)
        {
            return m1.Equals(m2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="m1">The first Matrix.</param>
        /// <param name="m2">The second Matrix.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(mat4 m1, mat4 m2)
        {
            return !m1.Equals(m2);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this[0].GetHashCode() ^ this[1].GetHashCode() ^ this[2].GetHashCode() ^ this[3].GetHashCode();
        }

        #endregion
    }
}
