using Fasciculus.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fasciculus.Mathematics
{
    public class DenseIntVector
    {
        private readonly int[] entries;

        public DenseIntVector(int[] entries)
        {
            this.entries = entries.ShallowCopy();
        }

        public int Count
            => entries.Length;

        public int this[int index]
            => entries[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DenseIntVector operator +(DenseIntVector lhs, DenseIntVector rhs)
            => new(Enumerable.Range(0, lhs.Count).Select(index => lhs.entries[index] + rhs.entries[index]).ToArray());
    }

    public class SparseBoolVector
    {
        private readonly BitSet entries;

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
    }
}
