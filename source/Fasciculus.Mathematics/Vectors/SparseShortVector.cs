using Fasciculus.Algorithms;
using Fasciculus.IO.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fasciculus.Mathematics.Vectors
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
                int i = BinarySearchUnsafe.IndexOf(indices, index);

                return i >= 0 ? values[i] : default;
            }
        }

        /// <summary>
        /// Initializes a vector from the given <paramref name="entries"/>.
        /// </summary>
        public SparseShortVector(Dictionary<uint, short> entries)
        {
            KeyValuePair<uint, short>[] sorted = [.. entries.Where(x => x.Value != 0).OrderBy(x => x.Key)];

            indices = [.. sorted.Select(x => x.Key)];
            values = [.. sorted.Select(x => x.Value)];
        }

        /// <summary>
        /// Initializes a vector from the given binary data.
        /// </summary>
        public SparseShortVector(Stream stream)
        {
            indices = stream.ReadUInt32Array();
            values = stream.ReadInt16Array();
        }

        /// <summary>
        /// Writes the vector to the given binary data.
        /// </summary>
        public void Write(Stream stream)
        {
            stream.WriteUInt32Array(indices);
            stream.WriteInt16Array(values);
        }

        /// <summary>
        /// Adds the given vectors <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// </summary>
        public static SparseShortVector operator +(SparseShortVector lhs, SparseShortVector rhs)
        {
            Dictionary<uint, short> entries = [];
            IEnumerable<uint> indices = lhs.indices.Concat(rhs.Indices).Distinct();

            foreach (uint i in indices)
            {
                entries[i] = (short)(lhs[i] + rhs[i]);
            }

            return new(entries);
        }
    }
}
