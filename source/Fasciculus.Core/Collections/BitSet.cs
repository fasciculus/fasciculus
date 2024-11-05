using Fasciculus.Algorithms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fasciculus.Collections
{
    public class BitSet : IEnumerable<int>
    {
        private readonly int[] entries;

        public int Count => entries.Length;

        internal BitSet(int[] entries)
        {
            this.entries = entries;
        }

        public BitSet(SortedSet<int> entries)
            : this(entries.ToArray()) { }

        public BitSet(IEnumerable<int> entries)
            : this(new SortedSet<int>(entries)) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet Create(SortedSet<int> entries)
            => new(entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet Create(IEnumerable<int> entries)
            => new(entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet Create(params int[] entries)
            => new(entries);

        public bool this[int value]
            => BinarySearch.IndexOf(entries, value) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(BitSet other)
            => SetOperations.Intersects(entries, other.entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator +(BitSet a, BitSet b)
            => new(SetOperations.Union(a.entries, b.entries));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator -(BitSet a, BitSet b)
            => new(SetOperations.Difference(a.entries, b.entries));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator *(BitSet a, BitSet b)
            => new(SetOperations.Intersection(a.entries, b.entries));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<int> GetEnumerator()
            => entries.AsEnumerable().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => entries.GetEnumerator();
    }
}
