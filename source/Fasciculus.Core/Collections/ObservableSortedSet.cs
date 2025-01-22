using Fasciculus.Threading.Synchronization;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Fasciculus.Collections
{
    /// <summary>
    /// Observable and task-safe sorted set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableSortedSet<T> : TaskSafeSortedSet<T>, INotifyPropertyChanged, INotifyingEnumerable<T>
        where T : notnull
    {
        private readonly TaskSafeMutex mutex = new();

        /// <summary>
        /// Notifies clients that a property value has changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifies clients that the collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Initializes a set with the given <paramref name="collection"/> and the given <paramref name="comparer"/>.
        /// </summary>
        public ObservableSortedSet(IEnumerable<T> collection, IComparer<T> comparer)
            : base(collection, comparer) { }

        /// <summary>
        /// Initializes a set with the given <paramref name="collection"/>
        /// </summary>
        public ObservableSortedSet(IEnumerable<T> collection)
            : base(collection) { }

        /// <summary>
        /// Initializes a set with the given <paramref name="comparer"/>.
        /// </summary>
        public ObservableSortedSet(IComparer<T> comparer)
            : base(comparer) { }

        /// <summary>
        /// Initializes an empty set.
        /// </summary>
        public ObservableSortedSet() { }

        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        public override bool Add(T item)
        {
            using Locker locker = Locker.Lock(mutex);
            int index = DoAdd(item);

            if (index >= 0)
            {
                OnAdded(item, index);
            }

            return index >= 0;
        }

        /// <summary>
        /// Removes an element from the current set and returns a value to indicate if the element was successfully removed.
        /// </summary>
        public override bool Remove(T item)
        {
            using Locker locker = Locker.Lock(mutex);
            int index = DoRemove(item);

            if (index >= 0)
            {
                OnRemoved(item, index);
            }

            return index >= 0;
        }

        /// <summary>
        /// Removes all elements from the set.
        /// </summary>
        public override void Clear()
        {
            using Locker locker = Locker.Lock(mutex);

            base.Clear();
            OnCleared();
        }

        /// <summary>
        /// Modifies the set so that it contains all elements that are present in either the set or the <paramref name="other"/> collection.
        /// </summary>
        public override void UnionWith(IEnumerable<T> other)
        {
            using Locker locker = Locker.Lock(mutex);

            foreach (T item in other)
            {
                int index = DoAdd(item);

                if (index >= 0)
                {
                    OnAdded(item, index);
                }
            }
        }

        /// <summary>
        /// Modifies the set so that it contains all elements that are present in both the set and the <paramref name="other"/> collection.
        /// </summary>
        public override void IntersectWith(IEnumerable<T> other)
        {
            using Locker locker = Locker.Lock(mutex);
            T[] items = [.. this];

            foreach (T item in items)
            {
                if (!other.Contains(item))
                {
                    int index = DoRemove(item);

                    OnRemoved(item, index);
                }
            }
        }

        /// <summary>
        /// Removes all elements in <paramref name="other"/> from this set.
        /// </summary>
        public override void ExceptWith(IEnumerable<T> other)
        {
            using Locker locker = Locker.Lock(mutex);

            foreach (T item in other)
            {
                int index = DoRemove(item);

                if (index >= 0)
                {
                    OnRemoved(item, index);
                }
            }
        }

        /// <summary>
        /// Modifies the set so that it contains all elements that are present either in this set or in the <paramref name="other"/> collection
        /// but not in both.
        /// </summary>
        public override void SymmetricExceptWith(IEnumerable<T> other)
        {
            using Locker locker = Locker.Lock(mutex);
            SortedSet<T> intersection = new(this, Comparer);

            intersection.IntersectWith(other);

            foreach (T item in intersection)
            {
                int index = DoRemove(item);

                OnRemoved(item, index);
            }

            foreach (T item in other)
            {
                if (!intersection.Contains(item))
                {
                    int index = DoAdd(item);

                    OnAdded(item, index);
                }
            }
        }

        private int DoAdd(T item)
            => base.Add(item) ? IndexOf(item) : -1;

        private int DoRemove(T item)
        {
            int index = IndexOf(item);

            if (index >= 0)
            {
                base.Remove(item);
            }

            return index;
        }

        private void OnAdded(T item, int index)
        {
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
            OnPropertyChanged(nameof(Count));
        }

        private void OnRemoved(T item, int index)
        {
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
            OnPropertyChanged(nameof(Count));
        }

        private void OnCleared()
        {
            NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Reset);

            OnCollectionChanged(args);
            OnPropertyChanged(nameof(Count));
        }

        /// <summary>
        /// Called when a property of this set changed.
        /// </summary>
        protected virtual void OnPropertyChanged(string name)
        {
            PropertyChangedEventArgs args = new(name);

            PropertyChanged?.Invoke(this, args);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, T item, int index)
        {
            NotifyCollectionChangedEventArgs args = new(action, item, index);

            OnCollectionChanged(args);
        }

        /// <summary>
        /// Called when the contents of this set changed.
        /// </summary>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
            => CollectionChanged?.Invoke(this, args);
    }
}
