using Fasciculus.Algorithms.Searching;
using Fasciculus.IO.Binary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Fasciculus.Mathematics.Collections
{
    /// <summary>
    /// Immutable bit set.
    /// </summary>
    public class BitSet : IEnumerable<uint>
    {
        /// <summary>
        /// Empty bit set.
        /// </summary>
        public static BitSet Empty { get; } = new(Array.Empty<uint>());

        private readonly uint[] entries;

        /// <summary>
        /// Number of elements in the bit set.
        /// </summary>
        public int Count => entries.Length;

        /// <summary>
        /// Returns <c>true</c> if the bit set contains the given value.
        /// </summary>
        public bool this[uint value]
            => BinarySearch.IndexOf(entries, value) >= 0;

        private BitSet(uint[] entries)
        {
            this.entries = entries;
        }

        /// <summary>
        /// Initializes a new bit set with the given values.
        /// </summary>
        public BitSet(SortedSet<uint> entries)
            : this(entries.ToArray()) { }

        /// <summary>
        /// Initializes a new bit set with the given values.
        /// </summary>
        public BitSet(IEnumerable<uint> entries)
            : this(new SortedSet<uint>(entries)) { }

        /// <summary>
        /// Initializes new bit set from the given binary data.
        /// </summary>
        public BitSet(Stream stream)
            : this(stream.ReadUInt32Array()) { }

        /// <summary>
        /// Writes the vector to the given binary data.
        /// </summary>
        public void Write(Stream stream)
            => stream.WriteUInt32Array(entries);

        /// <summary>
        /// Returns <c>true</c> if this bit set shares a value with the given bit set.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(BitSet other)
            => Intersects(entries, other.entries);

        /// <summary>
        /// Returns the union of the given bit sets.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator +(BitSet a, BitSet b)
            => new(Union(a.entries, b.entries));

        /// <summary>
        /// Returns a bit set with the values that occur in the set <paramref name="a"/> but not in the set <paramref name="b"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator -(BitSet a, BitSet b)
            => new(Difference(a.entries, b.entries));

        /// <summary>
        /// Returns a bit set with values that occur in both given sets.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BitSet operator *(BitSet a, BitSet b)
            => new(Intersection(a.entries, b.entries));

        /// <summary>
        /// Returns an enumerator that iterates through this collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<uint> GetEnumerator()
            => entries.AsEnumerable().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => entries.GetEnumerator();

        private static bool Intersects(ReadOnlySpan<uint> a, ReadOnlySpan<uint> b)
        {
            int na = a.Length;
            int nb = b.Length;

            if (na == 0 || nb == 0)
            {
                return false;
            }

            if (a[0] > b[nb - 1])
            {
                return false;
            }

            if (b[0] > a[na - 1])
            {
                return false;
            }

            if (na < nb)
            {
                return na < nb >> 3 ? IntersectsBinary(a, na, b) : IntersectsLinear(a, na, b, nb);
            }
            else
            {
                return nb < na >> 3 ? IntersectsBinary(b, nb, a) : IntersectsLinear(b, nb, a, na);
            }
        }

        private static bool IntersectsBinary(ReadOnlySpan<uint> a, int na, ReadOnlySpan<uint> b)
        {
            for (int i = 0; i < na; ++i)
            {
                if (BinarySearch.IndexOf(b, a[i]) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IntersectsLinear(ReadOnlySpan<uint> a, int na, ReadOnlySpan<uint> b, int nb)
        {
            int i = 0;
            int j = 0;

            while (i < na && j < nb)
            {
                uint x = a[i];
                uint y = b[j];

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

        private static uint[] Union(ReadOnlySpan<uint> a, ReadOnlySpan<uint> b)
        {
            int na = a.Length;
            int nb = b.Length;
            uint[] c = new uint[na + nb];
            int i = 0;
            int j = 0;
            int k = 0;

            while (i < na && j < nb)
            {
                uint x = a[i];
                uint y = b[j];

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

            while (i < na)
            {
                c[k++] = a[i++];
            }

            while (j < nb)
            {
                c[k++] = b[j++];
            }

            return [.. c.Take(k)];
        }

        private static uint[] Difference(ReadOnlySpan<uint> a, ReadOnlySpan<uint> b)
        {
            int na = a.Length;
            int nb = b.Length;
            uint[] c = new uint[na];
            int i = 0;
            int j = 0;
            int k = 0;

            while (i < na && j < nb)
            {
                uint x = a[i];
                uint y = b[j];

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

            while (i < na)
            {
                c[k++] = a[i++];
            }

            return [.. c.Take(k)];
        }

        private static uint[] Intersection(ReadOnlySpan<uint> a, ReadOnlySpan<uint> b)
        {
            int na = a.Length;
            int nb = b.Length;
            uint[] c = new uint[Math.Min(na, nb)];
            int i = 0;
            int j = 0;
            int k = 0;

            while (i < na && j < nb)
            {
                uint x = a[i];
                uint y = b[j];

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

            return [.. c.Take(k)];
        }
    }
}
