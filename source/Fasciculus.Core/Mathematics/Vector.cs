using Fasciculus.Algorithms;
using Fasciculus.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fasciculus.Mathematics
{
    public class SparseShortVector
    {
        public static readonly SparseShortVector Empty = new([], []);

        private readonly int[] indices;
        private readonly short[] values;

        public IEnumerable<int> Indices
            => indices;

        public short this[int index]
        {
            get
            {
                index = BinarySearch.IndexOf(indices, index);

                return index >= 0 ? values[index] : default;
            }
        }

        internal SparseShortVector(int[] indices, short[] values)
        {
            this.indices = indices;
            this.values = values;
        }

        public SparseShortVector(Stream stream)
        {
            indices = stream.ReadIntArray();
            values = stream.ReadShortArray();
        }

        public void Write(Stream stream)
        {
            stream.WriteIntArray(indices);
            stream.WriteShortArray(values);
        }

        public static SparseShortVector Create(short[] source)
        {
            int n = source.Length;
            int[] indices = new int[n];
            short[] values = new short[n];
            int k = 0;

            for (int i = 0; i < n; ++i)
            {
                if (source[i] != 0)
                {
                    indices[k] = i;
                    values[k] = source[i];
                    ++k;
                }
            }

            return new(new Span<int>(indices, 0, k).ToArray(), new Span<short>(values, 0, k).ToArray());
        }

        public static SparseShortVector operator +(SparseShortVector lhs, SparseShortVector rhs)
        {
            SortedSet<int> indices = new(lhs.indices.Concat(rhs.indices));
            short[] values = indices.Select(i => (short)(lhs[i] + rhs[i])).ToArray();

            return new([.. indices], values);
        }
    }

    public class DenseShortVector
    {
        private readonly short[] entries;

        public int Count
            => entries.Length;

        public short this[int index]
            => entries[index];

        public DenseShortVector(short[] entries)
        {
            this.entries = entries.ShallowCopy();
        }

        public void Write(Stream stream)
        {
            stream.WriteShortArray(entries);
        }

        public static DenseShortVector Read(Stream stream)
        {
            short[] entries = stream.ReadShortArray();

            return new(entries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SparseShortVector ToSparse()
            => SparseShortVector.Create(entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DenseShortVector operator +(DenseShortVector lhs, DenseShortVector rhs)
            => new(Enumerable.Range(0, lhs.Count).Select(index => (short)(lhs.entries[index] + rhs.entries[index])).ToArray());
    }

    public class DenseIntVector
    {
        private readonly int[] entries;

        public int Count
            => entries.Length;

        public int this[int index]
            => entries[index];

        public DenseIntVector(int[] entries)
        {
            this.entries = entries.ShallowCopy();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DenseIntVector operator +(DenseIntVector lhs, DenseIntVector rhs)
            => new(Enumerable.Range(0, lhs.Count).Select(index => lhs.entries[index] + rhs.entries[index]).ToArray());
    }

    public class SparseBoolVector
    {
        private readonly BitSet entries;

        public static readonly SparseBoolVector Empty = new(BitSet.Empty);

        public IEnumerable<int> Indices
            => entries;

        private SparseBoolVector(BitSet entries)
        {
            this.entries = entries;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SparseBoolVector Create(BitSet entries)
            => new(entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector Create(IEnumerable<int> entries)
            => Create(BitSet.Create(entries));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector Create(params int[] entries)
            => Create(BitSet.Create(entries));

        public bool this[int index]
            => entries[index];

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
            int[] indices = v.Indices.ToArray();
            short[] values = indices.Select(_ => f).ToArray();

            return new(indices, values);
        }
    }
}
