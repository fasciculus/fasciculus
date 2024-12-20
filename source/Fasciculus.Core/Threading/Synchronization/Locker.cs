using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Fasciculus.Threading.Synchronization
{
    public class Locker : IDisposable
    {
        private readonly ILockable lockable;

        private Locker(ILockable lockable)
        {
            this.lockable = lockable;
        }

        ~Locker()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            lockable.Unlock();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Locker Lock(ILockable lockable, CancellationToken ctk = default)
        {
            lockable.Lock(ctk);

            return new(lockable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Locked(ILockable lockable, Action action, CancellationToken ctk = default)
        {
            using Locker locker = Lock(lockable, ctk);

            action();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Locked<T>(ILockable lockable, Func<T> func, CancellationToken ctk = default)
        {
            using Locker locker = Lock(lockable, ctk);

            return func();
        }
    }
}
