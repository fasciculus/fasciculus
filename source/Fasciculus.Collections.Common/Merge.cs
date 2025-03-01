using System;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Collections
{
    public static class EnumerableExtensions
    {
        public static SortedSet<T> Merge<T>(this IEnumerable<T> a, IEnumerable<T> b, IComparer<T> comparer, Func<T, T, T> merge)
        {
            T[] aa = [.. a.Order(comparer)];
            T[] ba = [.. b.Order(comparer)];

            return new(MergeCore(aa, ba, comparer, merge), comparer);
        }

        public static SortedSet<T> Merge<T>(this SortedSet<T> a, IEnumerable<T> b, Func<T, T, T> merge)
        {
            IComparer<T> comparer = a.Comparer;
            T[] aa = a.ToArray();
            T[] ba = [.. b.Order(comparer)];

            return new(MergeCore(aa, ba, comparer, merge), comparer);
        }

        private static T[] MergeCore<T>(T[] a, T[] b, IComparer<T> comparer, Func<T, T, T> merge)
        {
            T[] c = new T[a.Length + b.Length];

            int na = a.Length;
            int nb = b.Length;

            int i = 0;
            int j = 0;
            int k = 0;

            while (i < na && j < nb)
            {
                T x = a[i];
                T y = b[j];
                int z = comparer.Compare(x, y);

                if (z == 0)
                {
                    c[k++] = merge(x, y);
                    ++i;
                    ++j;
                }
                else
                {
                    if (z < 0)
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
    }
}