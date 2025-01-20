using Fasciculus.Threading.Synchronization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Collections
{
    /// <summary>
    /// A task safe sorted set.
    /// </summary>
    public class TaskSafeSortedSet<T> : ISet<T>, ICollection<T>, IReadOnlyCollection<T>, ICollection, IEnumerable<T>, IEnumerable
    {
        private readonly TaskSafeMutex mutex = new();
        private readonly SortedSet<T> values;

        /// <summary>
        /// The number of elements in the set.
        /// </summary>
        public int Count => Locker.Locked(mutex, () => values.Count);

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Always <c>false</c>.
        /// </summary>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// Always <c>this</c>.
        /// </summary>
        object ICollection.SyncRoot => this;

        /// <summary>
        /// The comparer used to sort the set.
        /// </summary>
        public IComparer<T> Comparer => values.Comparer;

        /// <summary>
        /// Initializes a set with the given <paramref name="collection"/> and the given <paramref name="comparer"/>.
        /// </summary>
        public TaskSafeSortedSet(IEnumerable<T> collection, IComparer<T> comparer)
        {
            values = new(collection, comparer);
        }

        /// <summary>
        /// Initializes a set with the given <paramref name="collection"/>
        /// </summary>
        public TaskSafeSortedSet(IEnumerable<T> collection)
            : this(collection, Comparer<T>.Default) { }

        /// <summary>
        /// Initializes a set with the given <paramref name="comparer"/>.
        /// </summary>
        public TaskSafeSortedSet(IComparer<T> comparer)
            : this([], comparer) { }

        /// <summary>
        /// Initializes an empty set.
        /// </summary>
        public TaskSafeSortedSet()
            : this([], Comparer<T>.Default) { }

        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        public virtual bool Add(T item)
            => Locker.Locked(mutex, () => values.Add(item));

        void ICollection<T>.Add(T item)
            => Add(item);

        /// <summary>
        /// Removes an element from the current set and returns a value to indicate if the element was successfully removed.
        /// </summary>
        public virtual bool Remove(T item)
            => Locker.Locked(mutex, () => values.Remove(item));

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public virtual void Clear()
            => Locker.Locked(mutex, () => values.Clear());

        /// <summary>
        /// Determines whether the set contains a specific element.
        /// </summary>
        public bool Contains(T item)
             => Locker.Locked(mutex, () => values.Contains(item));

        /// <summary>
        /// Copies the set into the given <paramref name="array"/>.
        /// <para>
        /// Note: the <paramref name="array"/> must be large enough to hold <see cref="Count"/> values.
        /// </para>
        /// </summary>
        public void CopyTo(T[] array)
            => Locker.Locked(mutex, () => values.CopyTo(array));

        /// <summary>
        /// Copies the set into the given <paramref name="array"/> starting at array position <paramref name="index"/>.
        /// <para>
        /// Note: the <paramref name="array"/> must be large enough to hold <paramref name="index"/> + <see cref="Count"/> values.
        /// </para>
        /// </summary>
        public void CopyTo(T[] array, int index)
            => Locker.Locked(mutex, () => values.CopyTo(array, index));

        /// <summary>
        /// Copies <paramref name="count"/> values from the set into the given <paramref name="array"/>
        /// starting at array position <paramref name="index"/>.
        /// <para>
        /// Note: the <paramref name="array"/> must be large enough to hold <paramref name="index"/> + <paramref name="count"/> values.
        /// </para>
        /// </summary>
        public void CopyTo(T[] array, int index, int count)
            => Locker.Locked(mutex, () => values.CopyTo(array, index, count));

        void ICollection.CopyTo(Array array, int index)
            => Locker.Locked(mutex, () => (values as ICollection).CopyTo(array, index));

        /// <summary>
        /// Modifies the set so that it contains all elements that are present in either the set or the <paramref name="other"/> collection.
        /// </summary>
        public virtual void UnionWith(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.UnionWith(other));

        /// <summary>
        /// Modifies the set so that it contains all elements that are present in both the set and the <paramref name="other"/> collection.
        /// </summary>
        public virtual void IntersectWith(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.IntersectWith(other));

        /// <summary>
        /// Removes all elements in <paramref name="other"/> from this set.
        /// </summary>
        public virtual void ExceptWith(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.ExceptWith(other));

        /// <summary>
        /// Modifies the set so that it contains all elements that are present either in this set or in the <paramref name="other"/> collection
        /// but not in both.
        /// </summary>
        public virtual void SymmetricExceptWith(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.SymmetricExceptWith(other));

        /// <summary>
        /// Determines whether this set is a subset of the <paramref name="other"/> collection.
        /// </summary>
        public bool IsSubsetOf(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.IsSubsetOf(other));

        /// <summary>
        /// Determines whether this set is a proper subset of the <paramref name="other"/> collection.
        /// </summary>
        public bool IsProperSubsetOf(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.IsProperSubsetOf(other));

        /// <summary>
        /// Determines whether this set is a superset of the <paramref name="other"/> collection.
        /// </summary>
        public bool IsSupersetOf(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.IsSupersetOf(other));

        /// <summary>
        /// Determines whether this set is a proper superset of the <paramref name="other"/> collection.
        /// </summary>
        public bool IsProperSupersetOf(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.IsProperSupersetOf(other));

        /// <summary>
        /// Determines whether this set and the <paramref name="other"/> collection contain the same elements.
        /// </summary>
        public bool SetEquals(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.SetEquals(other));

        /// <summary>
        /// Determines whether this set and the <paramref name="other"/> collection share common elements.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Overlaps(IEnumerable<T> other)
            => Locker.Locked(mutex, () => values.Overlaps(other));

        private T[] GetArray()
            => Locker.Locked(mutex, () => values.ToArray());

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
            => GetArray().AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetArray().GetEnumerator();

        /// <summary>
        /// Returns the index of the given <paramref name="item"/> in the set.
        /// </summary>
        public int IndexOf(T item)
        {
            T[] array = GetArray();

            return Array.BinarySearch(array, 0, array.Length, item, Comparer);
        }
    }
}
