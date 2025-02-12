using Fasciculus.IO.Binary;
using Fasciculus.Mathematics.Vectors;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Mathematics.Matrices
{
    /// <summary>
    /// Sparse matrix of <see cref="bool"/>.
    /// </summary>
    public class SparseBoolMatrix
    {
        private readonly Dictionary<uint, SparseBoolVector> rows;

        /// <summary>
        /// Returns the row at the given <paramref name="index"/>.
        /// </summary>
        public SparseBoolVector this[uint index]
            => rows.TryGetValue(index, out SparseBoolVector? row) ? row : SparseBoolVector.Empty;

        /// <summary>
        /// Initializes a new matrix with the given <paramref name="rows"/>.
        /// </summary>
        public SparseBoolMatrix(Dictionary<uint, SparseBoolVector> rows)
        {
            this.rows = rows.Where(r => r.Value.Length()).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Initializes new matrix from the given binary data.
        /// </summary>
        public SparseBoolMatrix(Stream stream)
            : this(stream.ReadDictionary(s => s.ReadUInt32(), s => new SparseBoolVector(s))) { }

        /// <summary>
        /// Writes the matrix to the given binary data.
        /// </summary>
        public void Write(Stream stream)
        {
            stream.WriteDictionary(rows, (s, k) => s.WriteUInt32(k), (s, v) => { v.Write(s); });
        }

        /// <summary>
        /// Returns matrix-vector multiplication.
        /// </summary>
        public static SparseBoolVector operator *(SparseBoolMatrix matrix, SparseBoolVector vector)
            => new(matrix.rows.Keys.Where(i => matrix[i] * vector));
    }
}
