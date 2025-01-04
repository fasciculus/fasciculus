using Fasciculus.Collections;
using Fasciculus.IO;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Mathematics.LinearAlgebra
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
            this.rows = rows.Where(r => r.Value.Length()).ToDictionary();
        }

        /// <summary>
        /// Initializes new matrix from the given binary data.
        /// </summary>
        public SparseBoolMatrix(Binary bin)
            : this(bin.ReadDictionary(bin.ReadUInt, () => new SparseBoolVector(bin))) { }

        /// <summary>
        /// Writes the matrix to the given binary data.
        /// </summary>
        public void Write(Binary bin)
        {
            bin.WriteDictionary(rows, bin.WriteUInt, v => { v.Write(bin); });
        }

        /// <summary>
        /// Returns matrix-vector multiplication.
        /// </summary>
        public static SparseBoolVector operator *(SparseBoolMatrix matrix, SparseBoolVector vector)
            => new(matrix.rows.Keys.Where(i => matrix[i] * vector));
    }
}
