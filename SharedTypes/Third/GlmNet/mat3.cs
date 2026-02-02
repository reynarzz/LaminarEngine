using System;

namespace GlmNet
{
    /// <summary>
    /// Represents a 3x3 matrix.
    /// </summary>
    public struct mat3
    {
        public vec3 c0, c1, c2;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="mat3"/> struct.
        /// This matrix is the identity matrix scaled by <paramref name="scale"/>.
        /// </summary>
        /// <param name="scale">The scale.</param>
        public mat3(float scale)
        {
            c0 = new vec3(scale, 0.0f, 0.0f);
            c1 = new vec3(0.0f, scale, 0.0f);
            c2 = new vec3(0.0f, 0.0f, scale);
        }

        /// <summary>
        /// Constructs a 3x3 matrix from 9 scalars (column-major).
        /// </summary>
        public mat3(
            float m00, float m01, float m02,
            float m10, float m11, float m12,
            float m20, float m21, float m22)
        {
            c0 = new vec3(m00, m10, m20);
            c1 = new vec3(m01, m11, m21);
            c2 = new vec3(m02, m12, m22);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="mat3"/> struct.
        /// The matrix is initialised with the <paramref name="cols"/>.
        /// </summary>
        /// <param name="cols">The colums of the matrix.</param>
        public mat3(vec3[] cols)
        {
            c0 = cols[0];
            c1 = cols[1];
            c2 = cols[2];
        }

        public mat3(vec3 a, vec3 b, vec3 c)
        {
            c0 = a;
            c1 = b;
            c2 = c;
        }

        /// <summary>
        /// Creates an identity matrix.
        /// </summary>
        /// <returns>A new identity matrix.</returns>
        public static mat3 identity()
        {
            return new mat3(
                new vec3(1, 0, 0),
                new vec3(0, 1, 0),
                new vec3(0, 0, 1)
            );
        }

        #endregion

        #region Index Access

        /// <summary>
        /// Gets or sets the <see cref="vec3"/> column at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="vec3"/> column.
        /// </value>
        /// <param name="column">The column index.</param>
        /// <returns>The column at index <paramref name="column"/>.</returns>
        public vec3 this[int column]
        {
            get
            {
                return column switch
                {
                    0 => c0,
                    1 => c1,
                    2 => c2,
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
        private static readonly float[] _tempArray = new float[9];
        public float[] to_array()
        {
            _tempArray[0] = c0.x; _tempArray[1] = c0.y; _tempArray[2] = c0.z;
            _tempArray[3] = c1.x; _tempArray[4] = c1.y; _tempArray[5] = c1.z;
            _tempArray[6] = c2.x; _tempArray[7] = c2.y; _tempArray[8] = c2.z;
            return _tempArray;
        }

        public void to_array(Span<float> destination)
        {
            if (destination.Length < 9)
                throw new ArgumentException("Destination span must be at least 9 elements long.");

            destination[0] = c0.x; destination[1] = c0.y; destination[2] = c0.z;
            destination[3] = c1.x; destination[4] = c1.y; destination[5] = c1.z;
            destination[6] = c2.x; destination[7] = c2.y; destination[8] = c2.z;
        }

        /// <summary>
        /// Returns the <see cref="mat3"/> portion of this matrix.
        /// </summary>
        /// <returns>The <see cref="mat3"/> portion of this matrix.</returns>
        public mat2 to_mat2()
        {
            return new mat2(
                new vec2(c0.x, c0.y),
                new vec2(c1.x, c1.y)
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
        public static vec3 operator *(mat3 lhs, vec3 rhs)
        {
            return new vec3(
                lhs.c0.x * rhs.x + lhs.c1.x * rhs.y + lhs.c2.x * rhs.z,
                lhs.c0.y * rhs.x + lhs.c1.y * rhs.y + lhs.c2.y * rhs.z,
                lhs.c0.z * rhs.x + lhs.c1.z * rhs.y + lhs.c2.z * rhs.z
            );
        }

        /// <summary>
        /// Multiplies the <paramref name="lhs"/> matrix by the <paramref name="rhs"/> matrix.
        /// </summary>
        /// <param name="lhs">The LHS matrix.</param>
        /// <param name="rhs">The RHS matrix.</param>
        /// <returns>The product of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        public static mat3 operator *(mat3 lhs, mat3 rhs)
        {
            mat3 result = new mat3();
            result.c0 = lhs * rhs.c0;
            result.c1 = lhs * rhs.c1;
            result.c2 = lhs * rhs.c2;
            return result;
        }

        public static mat3 operator *(mat3 lhs, float s)
        {
            return new mat3(lhs.c0 * s, lhs.c1 * s, lhs.c2 * s);
        }

        #endregion

        #region ToString support

        public override string ToString()
        {
            return String.Format(
                "[{0}, {1}, {2}; {3}, {4}, {5}; {6}, {7}, {8}]",
                this[0, 0], this[1, 0], this[2, 0],
                this[0, 1], this[1, 1], this[2, 1],
                this[0, 2], this[1, 2], this[2, 2]
            );
        }
        #endregion

        #region comparision
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
            if (obj.GetType() == typeof(mat3))
            {
                var mat = (mat3)obj;
                if (mat[0] == this[0] && mat[1] == this[1] && mat[2] == this[2])
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
        public static bool operator ==(mat3 m1, mat3 m2)
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
        public static bool operator !=(mat3 m1, mat3 m2)
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
            return this[0].GetHashCode() ^ this[1].GetHashCode() ^ this[2].GetHashCode();
        }
        #endregion
    }
}
