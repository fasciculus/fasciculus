using System;
using System.Collections.Generic;

namespace Fasciculus.Collections
{
    public class DisposableStack<T> : Stack<T>, IDisposable
        where T : notnull, IDisposable
    {
        public DisposableStack() { }
        public DisposableStack(IEnumerable<T> collection) : base(collection) { }
        public DisposableStack(int capacity) : base(capacity) { }

        ~DisposableStack()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            while (Count > 0)
            {
                Pop().Dispose();
            }
        }
    }
}
