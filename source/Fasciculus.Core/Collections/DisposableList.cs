using System;
using System.Collections.Generic;

namespace Fasciculus.Collections
{
    /// <summary>
    /// Disposable list of disposable entries. The stack calls <see cref="IDisposable.Dispose()"/> on its entries if it itself gets disposed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisposableList<T> : List<T>, IDisposable
        where T : notnull, IDisposable
    {
        /// <summary>
        /// Initializes an empty list.
        /// </summary>
        public DisposableList() { }

        /// <summary>
        /// Initializes a list that contains elements copied from the specified collection.
        /// </summary>
        public DisposableList(IEnumerable<T> collection)
            : base(collection) { }

        /// <summary>
        /// Initializes an empty stack that has the specified initial <paramref name="capacity"/> or the default initial capacity,
        /// whichever is greater.
        /// </summary>
        public DisposableList(int capacity)
            : base(capacity) { }

        /// <summary>
        /// Disposes the list entries.
        /// </summary>
        ~DisposableList()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes and removes the list entries.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            this.Apply(x => x.Dispose());
            Clear();
        }
    }
}
