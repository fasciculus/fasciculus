using Fasciculus.IO;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Mathematics
{
    /// <summary>
    /// Sparse matrix of <see cref="short"/>.
    /// </summary>
    public class SparseShortMatrix
    {
        private readonly Dictionary<uint, SparseShortVector> rows;

        /// <summary>
        /// Returns the row at the given <paramref name="index"/>.
        /// </summary>
        public SparseShortVector this[uint index]
            => rows.TryGetValue(index, out SparseShortVector? row) ? row : SparseShortVector.Empty;

        /// <summary>
        /// Initializes a new matrix with the given <paramref name="rows"/>.
        /// </summary>
        public SparseShortMatrix(Dictionary<uint, SparseShortVector> rows)
        {
            this.rows = rows.Where(r => r.Value.Indices.Any()).ToDictionary();
        }

        /// <summary>
        /// Initializes new matrix from the given binary data.
        /// </summary>
        public SparseShortMatrix(Binary bin)
            : this(bin.ReadDictionary(bin.ReadUInt, () => new SparseShortVector(bin))) { }

        /// <summary>
        /// Writes the matrix to the given binary data.
        /// </summary>
        public void Write(Binary bin)
            => bin.WriteDictionary(rows, bin.WriteUInt, v => v.Write(bin));
    }
}
