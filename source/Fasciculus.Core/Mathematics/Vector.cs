using Fasciculus.Collections;
using Fasciculus.Validating;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Mathematics
{
    public readonly struct VectorEntry<T>
    {
        public int Index { get; init; }
        public T Value { get; init; }

        public VectorEntry(int index, T value)
        {
            Index = index;
            Value = value;
        }

        public static VectorEntry<T> Create(int index, T value)
            => new(index, value);
    }

    public abstract class Vector<T> : IEnumerable<VectorEntry<T>>
    {
        public abstract T this[int index] { get; }

        public abstract T Length();

        public abstract Vector<T> Add(Vector<T> vector);
        public abstract Vector<T> Sub(Vector<T> vector);

        protected abstract IEnumerable<VectorEntry<T>> GetVectorEntries();

        public IEnumerator<VectorEntry<T>> GetEnumerator()
            => GetVectorEntries().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetVectorEntries().GetEnumerator();

        public static Vector<T> operator +(Vector<T> a, Vector<T> b)
            => a.Add(b);

        public static Vector<T> operator -(Vector<T> a, Vector<T> b)
            => a.Sub(b);
    }

    public class SparseBoolVector : Vector<bool>
    {
        internal readonly BitSet entries;

        private SparseBoolVector(BitSet entries)
        {
            this.entries = entries;
        }

        public override bool this[int index]
            => entries[index];

        public override bool Length()
            => entries.Count > 0;

        public override Vector<bool> Add(Vector<bool> vector)
            => throw Ex.NotImplemented();

        public override Vector<bool> Sub(Vector<bool> vector)
            => throw Ex.NotImplemented();

        protected override IEnumerable<VectorEntry<bool>> GetVectorEntries()
            => entries.Select(index => VectorEntry<bool>.Create(index, true));

        public static SparseBoolVector Create(BitSet entries)
            => new(entries);

        public static SparseBoolVector Create(IEnumerable<int> entries)
            => Create(BitSet.Create(entries));

        public static SparseBoolVector operator +(SparseBoolVector a, SparseBoolVector b)
            => throw Ex.NotImplemented();

        public static SparseBoolVector operator -(SparseBoolVector a, SparseBoolVector b)
            => throw Ex.NotImplemented();
    }
}
