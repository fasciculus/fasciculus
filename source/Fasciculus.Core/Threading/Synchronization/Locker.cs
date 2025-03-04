using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Fasciculus.Threading.Synchronization
{
    /// <summary>
    /// Utility to lock/unlock synchronization objects.
    /// <para>
    /// Define a lockable:
    /// </para>
    /// <code>
    /// private readonly TaskSafeMutex mutex = new();
    /// </code>
    /// <para>
    /// Use it:
    /// </para>
    /// <code>
    /// using Locker locker = Locker.Lock(mutex);
    /// </code>
    /// </summary>
    public class Locker : IDisposable
    {
        private ILockable? lockable;

        private Locker(ILockable lockable)
        {
            this.lockable = lockable;
        }

        /// <summary>
        /// Finalizes this lock in case <see cref="Dispose()"/> hasn't been called.
        /// </summary>
        ~Locker()
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
        /// Locks the given synchronization object and returns a disposable "lock".
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Locker Lock(ILockable lockable, CancellationToken? ctk = null)
        {
            lockable.Lock(ctk.OrNone());

            return new(lockable);
        }

        /// <summary>
        /// Executes the given <paramref name="action"/> within a <see cref="ILockable.Lock(CancellationToken)"/> and
        /// <see cref="ILockable.Unlock"/> sequence.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Locked(ILockable lockable, Action action, CancellationToken? ctk = null)
        {
            using Locker locker = Lock(lockable, ctk);

            action();
        }

        /// <summary>
        /// Executes the given <paramref name="func"/> within a <see cref="ILockable.Lock(CancellationToken)"/> and
        /// <see cref="ILockable.Unlock"/> sequence.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Locked<T>(ILockable lockable, Func<T> func, CancellationToken? ctk = null)
        {
            using Locker locker = Lock(lockable, ctk);

            return func();
        }
    }
}
