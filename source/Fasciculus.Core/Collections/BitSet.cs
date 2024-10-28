using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Collections
{
    public class BitSet : IEnumerable<int>
    {
        internal readonly int[] entries;

        public int Count
            => entries.Length;

        private BitSet(int[] entries)
        {
            this.entries = entries;
        }

        public BitSet(SortedSet<int> entries)
            : this(entries.ToArray()) { }

        public BitSet(IEnumerable<int> entries)
            : this(new SortedSet<int>(entries)) { }

        public static BitSet Create(SortedSet<int> entries)
            => new(entries);

        public static BitSet Create(IEnumerable<int> entries)
            => new(entries);

        public static BitSet Create(params int[] entries)
            => new(entries);

        public bool this[int index]
            => Arrays.BinarySearch(entries, index) >= 0;

        public bool Intersects(BitSet other)
            => Intersects(entries, other.entries);

        public IEnumerator<int> GetEnumerator()
            => entries.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => entries.AsEnumerable().GetEnumerator();

        public static BitSet Union(BitSet a, BitSet b)
            => new(Union(a.entries, b.entries));

        public static BitSet operator +(BitSet a, BitSet b)
            => Union(a, b);

        public static BitSet Difference(BitSet a, BitSet b)
            => new(Difference(a.entries, b.entries));

        public static BitSet operator -(BitSet a, BitSet b)
            => Difference(a, b);

        public static BitSet Intersection(BitSet a, BitSet b)
            => new(Intersection(a.entries, b.entries));

        public static BitSet operator *(BitSet a, BitSet b)
            => Intersection(a, b);

        private static int[] Union(int[] a, int[] b)
        {
            int n = a.Length;
            int m = b.Length;
            int[] c = new int[n + m];
            int i = 0;
            int j = 0;
            int k = 0;

            while (i < n && j < m)
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

            while (i < n)
            {
                c[k++] = a[i++];
            }

            while (j < m)
            {
                c[k++] = b[j++];
            }

            return k < c.Length ? Arrays.SubArray(c, 0, k) : c;
        }

        private static int[] Difference(int[] a, int[] b)
        {
            int n = a.Length;
            int m = b.Length;
            int[] c = new int[n];
            int i = 0;
            int j = 0;
            int k = 0;

            while (i < n && j < m)
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

            while (i < n)
            {
                c[k++] = a[i++];
            }

            return k < c.Length ? Arrays.SubArray(c, 0, k) : c;
        }

        private static bool Intersects(int[] a, int[] b)
        {
            int n = a.Length;
            int m = b.Length;
            int i = 0;
            int j = 0;

            while (i < n && j < m)
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

        private static int[] Intersection(int[] a, int[] b)
        {
            int n = a.Length;
            int m = b.Length;
            int[] c = new int[Math.Min(n, m)];
            int i = 0;
            int j = 0;
            int k = 0;

            while (i < n && j < m)
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
