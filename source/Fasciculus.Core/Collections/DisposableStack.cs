using System;
using System.Collections.Generic;

namespace Fasciculus.Collections
{
    /// <summary>
    /// Disposable stack of disposable entries. The stack calls <see cref="IDisposable.Dispose()"/> on its entries if it itself gets disposed.
    /// </summary>
    public class DisposableStack<T> : Stack<T>, IDisposable
        where T : notnull, IDisposable
    {
        /// <summary>
        /// Initializes an empty stack.
        /// </summary>
        public DisposableStack() { }

        /// <summary>
        /// Initializes an empty stack that has the specified initial <paramref name="capacity"/> or the default initial capacity,
        /// whichever is greater.
        /// </summary>
        public DisposableStack(int capacity)
            : base(capacity) { }

        /// <summary>
        /// Initializes a stack that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection"></param>
        public DisposableStack(IEnumerable<T> collection)
            : base(collection) { }

        /// <summary>
        /// Disposes the stack's entries.
        /// </summary>
        ~DisposableStack()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes and removes the stack's entries.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            while (Count > 0)
            {
                Pop().Dispose();
            }
        }
    }
}
