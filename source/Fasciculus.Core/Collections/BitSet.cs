using Fasciculus.Mathematics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Collections
{
    public class BitSet : IEnumerable<int>
    {
        protected readonly Dictionary<int, byte> chunks;

        protected BitSet(Dictionary<int, byte> chunks)
        {
            this.chunks = chunks;
        }

        public int Count => Bits.Count(chunks.Values);

        public bool this[int index] => Contains(index);

        public bool Contains(int index)
        {
            int id = index >> 3;

            if (chunks.TryGetValue(id, out byte chunk))
            {
                byte mask = (byte)(1 << (index & 0x7));

                return ((chunk & mask) != 0);
            }

            return false;
        }

        public IEnumerator<int> GetEnumerator()
            => Enumerate();

        IEnumerator IEnumerable.GetEnumerator()
            => Enumerate();

        protected IEnumerator<int> Enumerate()
        {
            foreach (var entry in chunks.OrderBy(e => e.Key))
            {
                int id = entry.Key;
                byte chunk = entry.Value;
                byte mask = 1;

                for (int i = 0; i < 8; ++i, mask <<= 1)
                {
                    if ((chunk & mask) != 0)
                    {
                        yield return id << 3 + i;
                    }
                }
            }
        }

        public static Factory Create()
            => new();

        public class Mutable : BitSet
        {
            public Mutable() : base(new()) { }

            public void Add(int index)
            {
                int id = index >> 3;
                byte mask = (byte)(1 << (index & 0x7));

                if (chunks.TryGetValue(id, out byte chunk))
                {
                    chunk |= mask;
                    chunks.Replace(id, chunk);
                }
                else
                {
                    chunks.Add(id, chunk);
                }
            }
        }

        public class Factory
        {
            private Mutable result = new();

            public Factory Add(int index)
            {
                result.Add(index);

                return this;
            }

            public BitSet Build()
                => new(result.chunks);
        }
    }
}
