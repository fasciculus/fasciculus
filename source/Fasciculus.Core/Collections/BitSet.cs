using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Collections
{
    public abstract class BitSet : IEnumerable<ulong>
    {
        public IEnumerator<ulong> GetEnumerator()
            => Enumerate();

        IEnumerator IEnumerable.GetEnumerator()
            => Enumerate();

        protected abstract IEnumerator<ulong> Enumerate();

        public static Factory Create()
            => new();

        public class Immutable : BitSet
        {
            private readonly ulong[] ids;
            private readonly byte[] data;

            internal Immutable(SortedSet<ulong> bits)
            {
                var groups = bits.GroupBy(index => index & 0xffff_ffff_ffff_fff8);

                int count = groups.Count();
                ids = new ulong[count];
                data = new byte[count];
            }

            protected override IEnumerator<ulong> Enumerate()
            {
                for (int i = 0, n = ids.Length; i < n; ++i)
                {
                    ulong id = ids[i];
                    byte bits = data[i];
                    byte mask = 1;

                    for (ulong j = 0; j < 8; ++j, mask <<= 1)
                    {
                        if ((bits & mask) != 0)
                        {
                            yield return id + j;
                        }
                    }
                }
            }
        }

        public class Mutable : BitSet
        {
            protected readonly SortedSet<ulong> bits = new();

            public void Set(ulong index, bool value = true)
            {
                if (value)
                {
                    bits.Add(index);
                }
                else
                {
                    bits.Remove(index);
                }
            }

            public Immutable ToImmutable()
                => new(bits);

            protected override IEnumerator<ulong> Enumerate()
                => bits.GetEnumerator();
        }

        public class Factory
        {
            private readonly Mutable result = new();

            public Factory Set(ulong index, bool value = true)
            {
                result.Set(index, value);

                return this;
            }

            public Immutable Build()
                => result.ToImmutable();
        }
    }
}
