using Fasciculus.Mathematics;
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
            public class Entry : IEnumerable<ulong>
            {
                public readonly ulong Id;
                public readonly byte Mask;

                public Entry(ulong id, byte mask)
                {
                    Id = id;
                    Mask = mask;
                }

                public IEnumerator<ulong> GetEnumerator()
                    => Enumerate();

                IEnumerator IEnumerable.GetEnumerator()
                    => Enumerate();

                public IEnumerator<ulong> Enumerate()
                    => Bits.Indices(Mask).Select(i => i + Id).GetEnumerator();

                public static ulong ToId(ulong index)
                    => index & 0xffff_ffff_ffff_fff8ul;

                public static byte ToMask(IEnumerable<ulong> indices)
                    => Bits.IndicesToByte(indices.Select(index => index & 0x7ul));

                public static Entry Create(IGrouping<ulong, ulong> group)
                    => new(group.Key, ToMask(group));

            }

            private readonly Entry[] entries;

            internal Immutable(SortedSet<ulong> values)
            {
                entries = values.GroupBy(Entry.ToId).Select(Entry.Create).ToArray();
            }

            protected override IEnumerator<ulong> Enumerate()
                => entries.SelectMany(e => e).GetEnumerator();
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
