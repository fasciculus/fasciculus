using Fasciculus.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fasciculus.Collections
{
    public class BitSet : IEnumerable<int>
    {
        private readonly int[] entries;
        private readonly int index;
        public int Count { get; }

        internal BitSet(int[] entries, int index, int count)
        {
            this.entries = entries;
            this.index = index;
            Count = count;
        }

        private BitSet(int[] entries)
            : this(entries, 0, entries.Length) { }

        public BitSet(SortedSet<int> entries)
            : this(entries.ToArray()) { }

        public BitSet(IEnumerable<int> entries)
            : this(new SortedSet<int>(entries)) { }

        internal static BitSet Create(int[] entries, int index, int count)
            => new(entries, index, count);

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
            => BinarySearch.IndexOf(entries, index, Count, value) >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(BitSet other)
            // => Intersects(entries, index, Count, other.entries, other.index, other.Count);
            => SetOperations.Intersects(new ReadOnlySpan<int>(entries, index, Count), new ReadOnlySpan<int>(other.entries, other.index, other.Count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<int> GetEnumerator()
            => entries.Skip(index).Take(Count).GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => entries.Skip(index).Take(Count).GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet Union(BitSet a, BitSet b)
            => new(Union(a.entries, a.index, a.Count, b.entries, b.index, b.Count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator +(BitSet a, BitSet b)
            => Union(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet Difference(BitSet a, BitSet b)
            => new(Difference(a.entries, a.index, a.Count, b.entries, b.index, b.Count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator -(BitSet a, BitSet b)
            => Difference(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet Intersection(BitSet a, BitSet b)
            => new(Intersection(a.entries, a.index, a.Count, b.entries, b.index, b.Count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator *(BitSet a, BitSet b)
            => Intersection(a, b);

        private static int[] Union(int[] a, int aIndex, int aCount, int[] b, int bIndex, int bCount)
        {
            int[] c = new int[aCount + bCount];
            int i = aIndex;
            int iEnd = i + aCount;
            int j = bIndex;
            int jEnd = j + bCount;
            int k = 0;

            while (i < iEnd && j < jEnd)
            {
                int x = a[i];
                int y = b[j];

                if (x == y)
                {
                    c[k++] = x;
                    ++i;
                    ++j;
                }
                else
                {
                    if (x < y)
                    {
                        c[k++] = x;
                        ++i;
                    }
                    else
                    {
                        c[k++] = y;
                        ++j;
                    }
                }
            }

            while (i < iEnd)
            {
                c[k++] = a[i++];
            }

            while (j < jEnd)
            {
                c[k++] = b[j++];
            }

            return k < c.Length ? Arrays.SubArray(c, 0, k) : c;
        }

        private static int[] Difference(int[] a, int aIndex, int aCount, int[] b, int bIndex, int bCount)
        {
            int[] c = new int[aCount];
            int i = aIndex;
            int iEnd = i + aCount;
            int j = bIndex;
            int jEnd = j + bCount;
            int k = 0;

            while (i < iEnd && j < jEnd)
            {
                int x = a[i];
                int y = b[j];

                if (x == y)
                {
                    ++i;
                    ++j;
                }
                else
                {
                    if (x < y)
                    {
                        c[k++] = x;
                        ++i;
                    }
                    else
                    {
                        ++j;
                    }
                }
            }

            while (i < iEnd)
            {
                c[k++] = a[i++];
            }

            return k < c.Length ? Arrays.SubArray(c, 0, k) : c;
        }

        private static bool Intersects(int[] a, int aIndex, int aCount, int[] b, int bIndex, int bCount)
        {
            int i = aIndex;
            int iEnd = i + aCount;
            int j = bIndex;
            int jEnd = j + bCount;

            while (i < iEnd && j < jEnd)
            {
                int x = a[i];
                int y = b[j];

                if (x == y)
                {
                    return true;
                }
                else
                {
                    if (x < y)
                    {
                        ++i;
                    }
                    else
                    {
                        ++j;
                    }
                }
            }

            return false;
        }

        private static int[] Intersection(int[] a, int aIndex, int aCount, int[] b, int bIndex, int bCount)
        {
            int[] c = new int[Math.Min(aCount, bCount)];
            int i = aIndex;
            int iEnd = i + aCount;
            int j = bIndex;
            int jEnd = j + bCount;
            int k = 0;

            while (i < iEnd && j < jEnd)
            {
                int x = a[i];
                int y = b[j];

                if (x == y)
                {
                    c[k++] = x;
                    ++i;
                    ++j;
                }
                else
                {
                    if (x < y)
                    {
                        ++i;
                    }
                    else
                    {
                        ++j;
                    }
                }
            }

            return k < c.Length ? Arrays.SubArray(c, 0, k) : c;
        }
    }
}
