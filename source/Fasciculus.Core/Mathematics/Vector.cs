using Fasciculus.Algorithms;
using Fasciculus.Collections;
using Fasciculus.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fasciculus.Mathematics
{
    public class SparseShortVector
    {
        public static readonly SparseShortVector Empty = new([], []);

        private readonly uint[] indices;
        private readonly short[] values;

        public IEnumerable<uint> Indices
            => indices;

        public short this[uint index]
        {
            get
            {
                int i = BinarySearch.IndexOf(indices, index);

                return i >= 0 ? values[i] : default;
            }
        }

        internal SparseShortVector(uint[] indices, short[] values)
        {
            this.indices = indices;
            this.values = values;
        }

        public SparseShortVector(Binary bin)
        {
            indices = bin.ReadUIntArray();
            values = bin.ReadShortArray();
        }

        public void Write(Binary bin)
        {
            bin.WriteUIntArray(indices);
            bin.WriteShortArray(values);
        }

        public static SparseShortVector Create(short[] source)
        {
            int n = source.Length;
            uint[] indices = new uint[n];
            short[] values = new short[n];
            int k = 0;

            for (uint i = 0; i < n; ++i)
            {
                if (source[i] != 0)
                {
                    indices[k] = i;
                    values[k] = source[i];
                    ++k;
                }
            }

            return new(new Span<uint>(indices, 0, k).ToArray(), new Span<short>(values, 0, k).ToArray());
        }

        public static SparseShortVector operator +(SparseShortVector lhs, SparseShortVector rhs)
        {
            SortedSet<uint> indices = new(lhs.indices.Concat(rhs.indices));
            short[] values = indices.Select(i => (short)(lhs[i] + rhs[i])).ToArray();

            return new([.. indices], values);
        }
    }

    public class SparseBoolVector
    {
        private readonly BitSet entries;

        public static readonly SparseBoolVector Empty = new(BitSet.Empty);

        public IEnumerable<uint> Indices
            => entries;

        public bool this[uint index]
            => entries[index];

        private SparseBoolVector(BitSet entries)
        {
            this.entries = entries;
        }

        public SparseBoolVector(IEnumerable<uint> entries)
            : this(new BitSet(entries)) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Length()
            => entries.Count > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector operator +(SparseBoolVector lhs, SparseBoolVector rhs)
            => new(lhs.entries + rhs.entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector operator -(SparseBoolVector lhs, SparseBoolVector rhs)
            => new(lhs.entries - rhs.entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator *(SparseBoolVector lhs, SparseBoolVector rhs)
            => lhs.entries.Intersects(rhs.entries);

        public static SparseShortVector operator *(SparseBoolVector v, short f)
        {
            uint[] indices = v.Indices.ToArray();
            short[] values = indices.Select(_ => f).ToArray();

            return new(indices, values);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseShortVector operator *(short f, SparseBoolVector v)
            => v * f;
    }
}
