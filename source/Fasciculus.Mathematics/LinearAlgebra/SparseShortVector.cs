using Fasciculus.Algorithms;
using Fasciculus.Collections;
using Fasciculus.IO;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Mathematics.LinearAlgebra
{
    /// <summary>
    /// Immutable sparse vector of <see cref="short"/>.
    /// </summary>
    public class SparseShortVector
    {
        /// <summary>
        /// Empty vector.
        /// </summary>
        public static readonly SparseShortVector Empty = new([]);

        private readonly uint[] indices;
        private readonly short[] values;

        /// <summary>
        /// Indices having a non-zero value.
        /// </summary>
        public IEnumerable<uint> Indices
            => indices;

        /// <summary>
        /// Value at the given <paramref name="index"/>.
        /// </summary>
        public short this[uint index]
        {
            get
            {
                int i = BinarySearch.IndexOf(indices, index);

                return i >= 0 ? values[i] : default;
            }
        }

        /// <summary>
        /// Initializes a vector from the given <paramref name="entries"/>.
        /// </summary>
        public SparseShortVector(Dictionary<uint, short> entries)
        {
            KeyValuePair<uint, short>[] sorted = [.. entries.Where(x => x.Value != 0).OrderBy(x => x.Key)];

            indices = sorted.Select(x => x.Key).ToArray();
            values = sorted.Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// Initializes a vector from the given binary data.
        /// </summary>
        public SparseShortVector(BinaryRW bin)
        {
            indices = bin.ReadUIntArray();
            values = bin.ReadShortArray();
        }

        /// <summary>
        /// Writes the vector to the given binary data.
        /// </summary>
        public void Write(BinaryRW bin)
        {
            bin.WriteUIntArray(indices);
            bin.WriteShortArray(values);
        }

        /// <summary>
        /// Adds the given vectors <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// </summary>
        public static SparseShortVector operator +(SparseShortVector lhs, SparseShortVector rhs)
        {
            Dictionary<uint, short> entries = [];
            IEnumerable<uint> indices = lhs.indices.Concat(rhs.Indices).Distinct();

            indices.Apply(x => { entries[x] = (short)(lhs[x] + rhs[x]); });

            return new(entries);
        }
    }
}
