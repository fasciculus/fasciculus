﻿using Fasciculus.Threading;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.Collections
{
    public class TaskSafeList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        private readonly List<T> list = [];
        private readonly TaskSafeMutex mutex = new();

        public int Count => Locker.Locked(mutex, () => list.Count);

        public bool IsReadOnly => false;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public T this[int index]
        {
            get => Locker.Locked(mutex, () => list[index]);
            set => Locker.Locked(mutex, () => list[index] = value);
        }
        public void Add(T item)
            => Locker.Locked(mutex, () => list.Add(item));

        public void Clear()
            => Locker.Locked(mutex, () => list.Clear());

        public bool Contains(T item)
            => Locker.Locked(mutex, () => list.Contains(item));

        public void CopyTo(T[] array, int arrayIndex)
            => Locker.Locked(mutex, () => list.CopyTo(array, arrayIndex));

        public bool Remove(T item)
            => Locker.Locked(mutex, () => list.Remove(item));

        public IEnumerator<T> GetEnumerator()
            => Locker.Locked(mutex, () => list.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator()
            => Locker.Locked(mutex, () => list.GetEnumerator());

        public int IndexOf(T item)
            => Locker.Locked(mutex, () => list.IndexOf(item));

        public void Insert(int index, T item)
            => Locker.Locked(mutex, () => list.Insert(index, item));

        public void RemoveAt(int index)
            => Locker.Locked(mutex, () => list.RemoveAt(index));

        public void CopyTo(Array array, int index)
            => Locker.Locked(mutex, () => list.CopyTo((T[])array, index));

        public int Add(object value)
        {
            using Locker locker = Locker.Lock(mutex);

            list.Add((T)value);

            return list.Count - 1;
        }

        public bool Contains(object value)
            => Locker.Locked(mutex, () => list.Contains((T)value));

        public int IndexOf(object value)
            => Locker.Locked(mutex, () => list.IndexOf((T)value));

        public void Insert(int index, object value)
            => Locker.Locked(mutex, () => list.Insert(index, (T)value));

        public void Remove(object value)
            => Locker.Locked(mutex, () => list.Remove((T)value));
    }
}