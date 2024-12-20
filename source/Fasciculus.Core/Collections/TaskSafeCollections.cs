using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fasciculus.Collections
{
    public class TaskSafeList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>,
        IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
        where T : notnull
    {
        protected class Enumerator : IEnumerator<T>
        {
            public T Current => throw new NotImplementedException();
            object IEnumerator.Current => throw new NotImplementedException();

            public void Dispose() { }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        private readonly List<T> list = [];
        private readonly ReentrantTaskSafeMutex mutex = new();

        public int Count => Locker.Locked(mutex, () => list.Count);

        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public object SyncRoot => mutex;
        public bool IsFixedSize => false;

        object? IList.this[int index]
        {
            get => GetAt(index);
            set
            {
                if (value is not null && value is T t)
                {
                    SetAt(index, t);
                }
            }
        }

        public T this[int index] { get => GetAt(index); set => SetAt(index, value); }

        private T GetAt(int index) => Locker.Locked(mutex, () => list[index]);
        private void SetAt(int index, T value) => Locker.Locked(mutex, () => list[index] = value);

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
            => new Enumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => new Enumerator();

        public int IndexOf(T item)
            => Locker.Locked(mutex, () => list.IndexOf(item));

        public void Insert(int index, T item)
            => Locker.Locked(mutex, () => list.Insert(index, item));

        public void RemoveAt(int index)
            => Locker.Locked(mutex, () => list.RemoveAt(index));

        public void CopyTo(Array array, int index)
            => Locker.Locked(mutex, () => list.CopyTo((T[])array, index));

        public int Add(object? value)
        {
            using Locker locker = Locker.Lock(mutex);

            if (value is not null)
            {
                list.Add((T)value);
            }

            return list.Count - 1;
        }

        public bool Contains(object? value)
            => value is not null && value is T t && Locker.Locked(mutex, () => list.Contains(t));

        public int IndexOf(object? value)
        {
            if (value is null)
            {
                return -1;
            }

            return value is T t ? Locker.Locked(mutex, () => list.IndexOf(t)) : -1;
        }

        public void Insert(int index, object? value)
        {
            if (value is not null && value is T t)
            {
                Locker.Locked(mutex, () => list.Insert(index, t));
            }
        }

        public void Remove(object? value)
        {
            if (value is not null && value is T t)
            {
                Locker.Locked(mutex, () => list.Remove(t));
            }
        }
    }
}
