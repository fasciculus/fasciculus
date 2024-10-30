using System;

namespace Fasciculus.Algorithms
{
    public static class SetOperations
    {
        public static bool Intersects(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        {
            int na = a.Length;
            int nb = b.Length;

            if (na == 0 || nb == 0)
            {
                return false;
            }

            if (na < nb)
            {
                if (na < (nb >> 3))
                {
                    return IntersectsBinary(a, b);
                }
                else
                {
                    return IntersectsLinear(a, b);
                }
            }
            else
            {
                if (nb < (na >> 3))
                {
                    return IntersectsBinary(b, a);
                }
                else
                {
                    return IntersectsLinear(a, b);
                }
            }
        }

        private static bool IntersectsBinary(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        {
            for (int i = 0, n = a.Length; i < n; ++i)
            {
                if (BinarySearch.IndexOf(b, a[i]) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IntersectsLinear(ReadOnlySpan<int> a, ReadOnlySpan<int> b)
        {
            int i = 0;
            int iEnd = a.Length;
            int j = 0;
            int jEnd = b.Length;

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
    }
}
