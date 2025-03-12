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
        public static async Task<AsyncLock> LockAsync(IAsyncLockable lockable, CancellationToken? cancellationToken = null)
        {
            await lockable.LockAsync(cancellationToken.OrNone());

            return new AsyncLock(lockable);
        }

        /// <summary>
        /// Locks the given <paramref name="lockable"/>.
        /// </summary>
        public static AsyncLock Lock(IAsyncLockable lockable, CancellationToken? cancellationToken = null)
        {
            return Tasks.Wait(LockAsync(lockable, cancellationToken));
        }

        /// <summary>
        /// Invokes the given <paramref name="action"/> while locking the given <paramref name="lockable"/>.
        /// </summary>
        public static async Task LockedAsync(IAsyncLockable lockable, Action action, CancellationToken? cancellationToken = null)
        {
            using AsyncLock asyncLock = await LockAsync(lockable, cancellationToken);

            action();
        }

        /// <summary>
        /// Invokes the given <paramref name="action"/> while locking the given <paramref name="lockable"/>.
        /// </summary>
        public static void Locked(IAsyncLockable lockable, Action action, CancellationToken? cancellationToken = null)
            => Tasks.Wait(LockedAsync(lockable, action, cancellationToken));

        /// <summary>
        /// Invokes the given <paramref name="func"/> while locking the given <paramref name="lockable"/>.
        /// </summary>
        public static async Task<T> LockedAsync<T>(IAsyncLockable lockable, Func<T> func, CancellationToken? cancellationToken = null)
        {
            using AsyncLock asyncLock = await LockAsync(lockable, cancellationToken);

            return func();
        }

        /// <summary>
        /// Invokes the given <paramref name="func"/> while locking the given <paramref name="lockable"/>.
        /// </summary>
        public static T Locked<T>(IAsyncLockable lockable, Func<T> func, CancellationToken? cancellationToken = null)
            => Tasks.Wait(LockedAsync(lockable, func, cancellationToken));
    }
}
