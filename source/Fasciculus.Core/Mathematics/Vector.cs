using Fasciculus.Collections;
using Fasciculus.Validating;
using System;
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

    public abstract class Vector<T, V> : IEnumerable<VectorEntry<T>>
        where T : notnull
        where V : Vector<T, V>
    {
        public abstract int Count { get; }
        public abstract IEnumerable<int> Indices { get; }

        public abstract T this[int index] { get; }

        public abstract T Length();

        public abstract V Add(V vector);
        public abstract V Sub(V vector);
        public abstract T Dot(V vector);

        protected abstract IEnumerable<VectorEntry<T>> GetVectorEntries();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<VectorEntry<T>> GetEnumerator()
            => GetVectorEntries().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => GetVectorEntries().GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator +(Vector<T, V> lhs, Vector<T, V> rhs)
            => ((V)lhs).Add((V)rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static V operator -(Vector<T, V> lhs, Vector<T, V> rhs)
            => ((V)lhs).Sub((V)rhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T operator *(Vector<T, V> lhs, Vector<T, V> rhs)
            => ((V)lhs).Dot((V)rhs);
    }

    public class DenseIntVector : Vector<int, DenseIntVector>
    {
        private readonly int[] entries;

        public DenseIntVector(int[] entries)
        {
            this.entries = entries.ShallowCopy();
        }

        public override int Count
            => entries.Length;

        public override IEnumerable<int> Indices
            => throw Ex.NotImplemented();

        public override int this[int index]
            => entries[index];

        public override int Length()
            => throw Ex.NotImplemented();

        public override DenseIntVector Add(DenseIntVector rhs)
            => new(Enumerable.Range(0, Count).Select(index => entries[index] + rhs.entries[index]).ToArray());

        public override DenseIntVector Sub(DenseIntVector vector)
            => throw Ex.NotImplemented();

        public override int Dot(DenseIntVector vector)
            => throw Ex.NotImplemented();

        protected override IEnumerable<VectorEntry<int>> GetVectorEntries()
            => throw Ex.NotImplemented();
    }

    public class SparseBoolVector : Vector<bool, SparseBoolVector>
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override SparseBoolVector Add(SparseBoolVector vector)
            => new(entries + vector.entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override SparseBoolVector Sub(SparseBoolVector vector)
            => new(entries - vector.entries);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Dot(SparseBoolVector rhs)
            => entries.Intersects(rhs.entries);

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
    }
}
