using Fasciculus.Collections;
using Fasciculus.Mathematics.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Fasciculus.Mathematics.Vectors
{
    /// <summary>
    /// Immutable sparse vector of <see cref="bool"/>.
    /// </summary>
    public class SparseBoolVector
    {
        private readonly BitSet entries;

        /// <summary>
        /// Empty vector.
        /// </summary>
        public static readonly SparseBoolVector Empty = new(BitSet.Empty);

        /// <summary>
        /// Indices having a non-false value.
        /// </summary>        
        public IEnumerable<uint> Indices
            => entries;

        /// <summary>
        /// Value at the given <paramref name="index"/>.
        /// </summary>
        public bool this[uint index]
            => entries[index];

        /// <summary>
        /// Initializes a vector from the given <paramref name="entries"/>.
        /// </summary>
        public SparseBoolVector(BitSet entries)
        {
            this.entries = entries;
        }

        /// <summary>
        /// Initializes a vector from the given <paramref name="entries"/>.
        /// </summary>
        public SparseBoolVector(SortedSet<uint> entries)
            : this(new BitSet(entries)) { }

        /// <summary>
        /// Initializes a vector from the given <paramref name="entries"/>.
        /// </summary>
        public SparseBoolVector(IEnumerable<uint> entries)
            : this(new BitSet(entries)) { }

        /// <summary>
        /// Initializes a vector from the given binary data.
        /// </summary>
        public SparseBoolVector(Stream stream)
            : this(new BitSet(stream)) { }

        /// <summary>
        /// Writes the vector to the given binary data.
        /// </summary>
        public void Write(Stream stream)
            => entries.Write(stream);

        /// <summary>
        /// The vector length <c>|v|</c> of this vector.
        /// <para>
        /// This is not the entry count.
        /// </para>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Length()
            => entries.Count > 0;

        /// <summary>
        /// Adds the given vectors <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector operator +(SparseBoolVector lhs, SparseBoolVector rhs)
            => new(lhs.entries + rhs.entries);

        /// <summary>
        /// Subtracts the given vector <paramref name="rhs"/> from the given vector <paramref name="lhs"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector operator -(SparseBoolVector lhs, SparseBoolVector rhs)
            => new(lhs.entries - rhs.entries);

        /// <summary>
        /// Returns the dot product of the given vectors <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator *(SparseBoolVector lhs, SparseBoolVector rhs)
            => lhs.entries.Intersects(rhs.entries);

        /// <summary>
        /// Returns a <see cref="SparseShortVector"/> that has all entries set to <paramref name="factor"/> where
        /// the entries of <paramref name="vector"/> are <c>true</c>.
        /// </summary>
        public static SparseShortVector operator *(SparseBoolVector vector, short factor)
        {
            Dictionary<uint, short> result = [];

            vector.Indices.Apply(i => { result[i] = factor; });

            return new(result);
        }

        /// <summary>
        /// Returns a <see cref="SparseShortVector"/> that has all entries set to <paramref name="factor"/> where
        /// the entries of <paramref name="vector"/> are <c>true</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseShortVector operator *(short factor, SparseBoolVector vector)
            => vector * factor;
    }
}
