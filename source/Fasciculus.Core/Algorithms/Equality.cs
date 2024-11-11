using System;

namespace Fasciculus.Algorithms
{
    public static class Equality
    {
        public static bool AreEqual(Span<byte> a, Span<byte> b)
        {
            int na = a.Length;
            int nb = b.Length;

            if (na != nb) return false;

            for (int i = 0; i < na; ++i)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }
    }
}
