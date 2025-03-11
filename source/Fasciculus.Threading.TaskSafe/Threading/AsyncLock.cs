using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    /// <summary>
    /// Represents a locked <see cref="IAsyncLockable"/>.
    /// </summary>
    public class AsyncLock : IDisposable
    {
        private IAsyncLockable? lockable;

        private AsyncLock(IAsyncLockable lockable)
        {
            this.lockable = lockable;
        }

        /// <summary>
        /// Finalizes this lock in case <see cref="Dispose()"/> hasn't been called.
        /// </summary>
        ~AsyncLock()
        {
            Dispose(false);
        }

        /// <summary>
        /// Unlocks the locked synchronization object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool _)
        {
            lockable?.Unlock();
            lockable = null;
        }

        /// <summary>
        /// Locks the given <paramref name="lockable"/>.
        /// </summary>
        public static async Task<AsyncLock> Lock(IAsyncLockable lockable, CancellationToken? cancellationToken = null)
        {
            await lockable.Lock(cancellationToken.OrNone());

            return new AsyncLock(lockable);
        }

        /// <summary>
        /// Invokes the given <paramref name="action"/> while locking the given <paramref name="lockable"/>.
        /// </summary>
        public static async Task Locked(IAsyncLockable lockable, Action action, CancellationToken? cancellationToken = null)
        {
            using AsyncLock asyncLock = await Lock(lockable, cancellationToken);

            action();
        }

        /// <summary>
        /// Invokes the given <paramref name="func"/> while locking the given <paramref name="lockable"/>.
        /// </summary>
        public static async Task<T> Locked<T>(IAsyncLockable lockable, Func<T> func, CancellationToken? cancellationToken = null)
        {
            using AsyncLock asyncLock = await Lock(lockable, cancellationToken);

            return func();
        }
    }
}
