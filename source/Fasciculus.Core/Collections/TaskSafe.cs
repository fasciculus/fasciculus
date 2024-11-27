using Fasciculus.Threading;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.Collections
{
    public class TaskSafeList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        private readonly List<T> list = [];
        private readonly TaskSafeMutex mutex = new();

        public int Count
        {
            get
            {
                using Locker locker = Locker.Lock(mutex);

                return list.Count;
            }
        }

        public bool IsReadOnly => false;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        object IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public T this[int index]
        {
            get
            {
                using Locker locker = Locker.Lock(mutex);

                return list[index];
            }
            set
            {
                using Locker locker = Locker.Lock(mutex);

                list[index] = value;
            }
        }

        public void Add(T item)
        {
            using Locker locker = Locker.Lock(mutex);

            list.Add(item);
        }

        public void Clear()
        {
            using Locker locker = Locker.Lock(mutex);

            list.Clear();
        }

        public bool Contains(T item)
        {
            using Locker locker = Locker.Lock(mutex);

            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            using Locker locker = Locker.Lock(mutex);

            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            using Locker locker = Locker.Lock(mutex);

            return list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            using Locker locker = Locker.Lock(mutex);

            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            using Locker locker = Locker.Lock(mutex);

            return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            using Locker locker = Locker.Lock(mutex);

            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            using Locker locker = Locker.Lock(mutex);

            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            using Locker locker = Locker.Lock(mutex);

            list.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            using Locker locker = Locker.Lock(mutex);

            list.CopyTo((T[])array, index);
        }

        public int Add(object value)
        {
            using Locker locker = Locker.Lock(mutex);

            list.Add((T)value);

            return list.Count - 1;
        }

        public bool Contains(object value)
        {
            using Locker locker = Locker.Lock(mutex);

            return list.Contains((T)value);
        }

        public int IndexOf(object value)
        {
            using Locker locker = Locker.Lock(mutex);

            return list.IndexOf((T)value);
        }

        public void Insert(int index, object value)
        {
            using Locker locker = Locker.Lock(mutex);

            list.Insert(index, (T)value);
        }

        public void Remove(object value)
        {
            using Locker locker = Locker.Lock(mutex);

            list.Remove((T)value);
        }
    }
}
