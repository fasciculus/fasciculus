using Fasciculus.Support;
using Fasciculus.Threading.Synchronization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Fasciculus.Collections
{
    /// <summary>
    /// A task-safe <see cref="ObservableCollection{T}"/> that wraps a <see cref="INotifyingEnumerable{T}"/>.
    /// <para>
    /// Do not call modifying methods like <c>Add</c>, <c>Clear</c>, <c>Move</c>, ... on this collection directly.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableNotifyingEnumerable<T> : ObservableCollection<T>
        where T : notnull
    {
        private readonly TaskSafeMutex mutex = new();
        private readonly INotifyCollectionChanged notifier;

        /// <summary>
        /// Initializes a collection with the given <paramref name="source"/>.
        /// <para>
        /// Note: <paramref name="source"/> and <paramref name="notifier"/> must be the same object.
        /// </para>
        /// </summary>
        public ObservableNotifyingEnumerable(IEnumerable<T> source, INotifyCollectionChanged notifier)
            : base(source)
        {
            if (!ReferenceEquals(source, notifier))
            {
                throw Ex.Argument();
            }

            this.notifier = notifier;
            this.notifier.CollectionChanged += OnSourceChanged;
        }

        /// <summary>
        /// Initializes a collection with the given <paramref name="source"/>.
        /// </summary>
        public ObservableNotifyingEnumerable(INotifyingEnumerable<T> source)
            : this(source, source) { }

        /// <summary>
        /// Destroys this collection.
        /// </summary>
        ~ObservableNotifyingEnumerable()
        {
            notifier.CollectionChanged -= OnSourceChanged;
        }

        private void OnSourceChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            TypedNotifyCollectionChangedEventArgs<T> ev = new(e);

            switch (ev.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnAdded(ev.NewItems, ev.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    OnRemoved(ev.OldItems, ev.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    OnReplaced(ev.OldItems, ev.NewItems, ev.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:
                    OnMoved(ev.NewItems, ev.OldStartingIndex, ev.NewStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    OnReset();
                    break;
            }
        }

        private void OnAdded(List<T> newItems, int startIndex)
        {
            using Locker locker = Locker.Lock(mutex);

            if (startIndex < 0)
            {
                startIndex = Count;
            }

            for (int i = 0, n = newItems.Count, index = startIndex; i < n; ++i, ++index)
            {
                base.InsertItem(index, newItems[i]);
            }

        }

        private void OnRemoved(List<T> oldItems, int startIndex)
        {
            using Locker locker = Locker.Lock(mutex);

            if (startIndex < 0)
            {
                foreach (T item in oldItems)
                {
                    int index = IndexOf(item);

                    if (index >= 0)
                    {
                        base.RemoveItem(index);
                    }
                }
            }
            else
            {
                for (int i = 0, n = oldItems.Count; i < n; ++i)
                {
                    base.RemoveItem(startIndex);
                }
            }
        }

        private void OnReplaced(List<T> oldItems, List<T> newItems, int startIndex)
        {
            using Locker locker = Locker.Lock(mutex);

            if (startIndex < 0)
            {
                for (int i = 0, n = oldItems.Count; i < n; ++i)
                {
                    int index = IndexOf(oldItems[i]);

                    if (index >= 0)
                    {
                        base.SetItem(index, newItems[i]);
                    }
                }
            }
            else
            {
                for (int i = 0, n = oldItems.Count, index = startIndex; i < n; ++i, ++index)
                {
                    base.SetItem(index, newItems[i]);
                }
            }
        }

        private void OnMoved(List<T> items, int oldIndex, int newIndex)
        {
            using Locker locker = Locker.Lock(mutex);

            for (int i = 0, n = items.Count; i < n; ++i)
            {
                base.MoveItem(oldIndex + i, newIndex + i);
            }
        }

        private void OnReset()
        {
            using Locker locker = Locker.Lock(mutex);

            base.ClearItems();
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/>
        /// </summary>
        protected override void ClearItems()
        {
            throw Ex.InvalidOperation();
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/>
        /// </summary>
        protected override void InsertItem(int index, T item)
        {
            throw Ex.InvalidOperation();
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/>
        /// </summary>
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw Ex.InvalidOperation();
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/>
        /// </summary>
        protected override void RemoveItem(int index)
        {
            throw Ex.InvalidOperation();
        }

        /// <summary>
        /// Throws a <see cref="InvalidOperationException"/>
        /// </summary>
        protected override void SetItem(int index, T item)
        {
            throw Ex.InvalidOperation();
        }
    }
}
