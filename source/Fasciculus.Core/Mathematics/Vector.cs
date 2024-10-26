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

    public interface IVector<T> : IEnumerable<VectorEntry<T>>
        where T : notnull
    {
        public T this[int index] { get; }

        public T Length();

        public IVector<T> Add(IVector<T> vector);
        public IVector<T> Sub(IVector<T> vector);
    }

    public interface IMutableVector<T> : IVector<T>
        where T : notnull
    {
    }

    public class SparseBoolVector : IVector<bool>
    {
        private readonly SortedSet<int> indices;

        private SparseBoolVector(SortedSet<int> indices)
        {
            this.indices = indices;
        }

        public bool this[int index]
            => indices.Contains(index);

        public bool Length()
            => indices.Count > 0;

        public IVector<bool> Add(IVector<bool> vector)
            => Create(indices.Concat(vector.Select(e => e.Index)));

        public IVector<bool> Sub(IVector<bool> vector)
            => Create(indices.Where(index => !vector[index]));

        private IEnumerable<VectorEntry<bool>> Entries()
            => indices.Select(index => VectorEntry<bool>.Create(index, true));

        public IEnumerator<VectorEntry<bool>> GetEnumerator()
            => Entries().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Entries().GetEnumerator();

        public static SparseBoolVector Create(IEnumerable<int> indices)
            => new(new(indices));
    }

    public static class Vectors
    {
        public static IVector<bool> CreateSparseBool(IEnumerable<int> indices)
            => SparseBoolVector.Create(indices);
    }
}
