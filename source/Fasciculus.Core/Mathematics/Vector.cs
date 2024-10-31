using Fasciculus.Collections;
using Fasciculus.Validating;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorEntry<T> Create(int index, T value)
            => new(index, value);
    }

    public abstract class Vector<T> : IEnumerable<VectorEntry<T>>
    {
        public abstract int Count { get; }
        public abstract IEnumerable<int> Indices { get; }

        public abstract T this[int index] { get; }

        public abstract T Length();

        public abstract Vector<T> Add(Vector<T> vector);
        public abstract Vector<T> Sub(Vector<T> vector);
        public abstract T Dot(Vector<T> vector);

        protected abstract IEnumerable<VectorEntry<T>> GetVectorEntries();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<VectorEntry<T>> GetEnumerator()
            => GetVectorEntries().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => GetVectorEntries().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> operator +(Vector<T> a, Vector<T> b)
            => a.Add(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector<T> operator -(Vector<T> a, Vector<T> b)
            => a.Sub(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator *(Vector<T> a, Vector<T> b)
            => a.Dot(b);
    }

    public class DenseIntVector : Vector<int>
    {
        internal DenseIntVector(int[] values)
        {
        }

        public override int Count
            => throw Ex.NotImplemented();

        public override IEnumerable<int> Indices
            => throw Ex.NotImplemented();

        public override int this[int index]
            => throw Ex.NotImplemented();

        public override int Length()
            => throw Ex.NotImplemented();

        public override Vector<int> Add(Vector<int> vector)
            => throw Ex.NotImplemented();

        public override Vector<int> Sub(Vector<int> vector)
            => throw Ex.NotImplemented();

        public override int Dot(Vector<int> vector)
            => throw Ex.NotImplemented();

        protected override IEnumerable<VectorEntry<int>> GetVectorEntries()
            => throw Ex.NotImplemented();
    }

    public class SparseBoolVector : Vector<bool>
    {
        private readonly BitSet entries;

        public override int Count
            => throw Ex.NotImplemented();

        public override IEnumerable<int> Indices
            => entries;

        private SparseBoolVector(BitSet entries)
        {
            this.entries = entries;
        }

        public override bool this[int index]
            => entries[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Length()
            => entries.Count > 0;

        public override Vector<bool> Add(Vector<bool> vector)
            => throw Ex.NotImplemented();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SparseBoolVector Add(SparseBoolVector vector)
            => new(entries + vector.entries);

        public override Vector<bool> Sub(Vector<bool> vector)
            => throw Ex.NotImplemented();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SparseBoolVector Sub(SparseBoolVector vector)
            => new(entries - vector.entries);

        public override bool Dot(Vector<bool> vector)
            => throw Ex.NotImplemented();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Dot(SparseBoolVector vector)
            => entries.Intersects(vector.entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override IEnumerable<VectorEntry<bool>> GetVectorEntries()
            => entries.Select(index => VectorEntry<bool>.Create(index, true));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static SparseBoolVector Create(BitSet entries)
            => new(entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector Create(IEnumerable<int> entries)
            => Create(BitSet.Create(entries));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector Create(params int[] entries)
            => Create(BitSet.Create(entries));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector operator +(SparseBoolVector a, SparseBoolVector b)
            => a.Add(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SparseBoolVector operator -(SparseBoolVector a, SparseBoolVector b)
            => a.Sub(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator *(SparseBoolVector a, SparseBoolVector b)
            => a.Dot(b);
    }
}
