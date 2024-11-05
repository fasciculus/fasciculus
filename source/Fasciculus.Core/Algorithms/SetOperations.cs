using System;

namespace Fasciculus.Algorithms
{
    public static class SetOperations
    {
        public static unsafe bool Intersects(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        {
            int na = a.Length;
            int nb = b.Length;

            if (na == 0)
            {
                return false;
            }

            if (nb == 0)
            {
                return false;
            }

            fixed (int* pa = a)
            {
                fixed (int* pb = b)
                {
                    if (pa[0] > pb[nb - 1])
                    {
                        return false;
                    }

                    if (pb[0] > pa[na - 1])
                    {
                        return false;
                    }

                    if (na < nb)
                    {
                        return na < nb >> 3 ? IntersectsBinary(pa, na, pb, nb) : IntersectsLinear(pa, na, pb, nb);
                    }
                    else
                    {
                        return nb < na >> 3 ? IntersectsBinary(pb, nb, pa, na) : IntersectsLinear(pa, na, pb, nb);
                    }
                }
            }
        }

        private static unsafe bool IntersectsBinary(int* pa, int na, int* pb, int nb)
        {
            for (int i = 0; i < na; ++i)
            {
                if (BinarySearch.IndexOf(pb, nb, pa[i]) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static unsafe bool IntersectsLinear(int* pa, int na, int* pb, int nb)
        {
            int i = 0;
            int j = 0;

            while (i < na && j < nb)
            {
                int x = pa[i];
                int y = pb[j];

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

        public static unsafe int[] Union(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        {
            int na = a.Length;
            int nb = b.Length;
            Span<int> c = stackalloc int[na + nb];
            int i = 0;
            int j = 0;
            int k = 0;

            fixed (int* pa = a)
            {
                fixed (int* pb = b)
                {
                    while (i < na && j < nb)
                    {
                        int x = pa[i];
                        int y = pb[j];

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
                        c[k++] = pa[i++];
                    }

                    while (j < nb)
                    {
                        c[k++] = pb[j++];
                    }
                }
            }

            return c[..k].ToArray();
        }

        public static unsafe int[] Difference(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        {
            int na = a.Length;
            int nb = b.Length;
            Span<int> c = stackalloc int[na];
            int i = 0;
            int j = 0;
            int k = 0;

            fixed (int* pa = a)
            {
                fixed (int* pb = b)
                {
                    while (i < na && j < nb)
                    {
                        int x = pa[i];
                        int y = pb[j];

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
                        c[k++] = pa[i++];
                    }
                }
            }

            return c[..k].ToArray();
        }

        public static unsafe int[] Intersection(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        {
            int na = a.Length;
            int nb = b.Length;
            Span<int> c = stackalloc int[Math.Min(na, nb)];
            int i = 0;
            int j = 0;
            int k = 0;

            fixed (int* pa = a)
            {
                fixed (int* pb = b)
                {
                    while (i < na && j < nb)
                    {
                        int x = pa[i];
                        int y = pb[j];

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
                }
            }

            return c[..k].ToArray();
        }
    }
}
