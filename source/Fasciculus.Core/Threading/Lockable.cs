using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    public interface ILockable
    {
        public void Lock(CancellationToken ctk);

        public void Unlock();
    }

    public class TaskSafeMutex : ILockable
    {
        private long locked = 0;

        public void Lock(CancellationToken ctk)
        {
            while (true)
            {
                ctk.ThrowIfCancellationRequested();

                if (Interlocked.CompareExchange(ref locked, 0, 1) == 0)
                {
                    return;
                }

                Task.Delay(1).RunSynchronously();
            }
        }

        public void Unlock()
        {
            Interlocked.Exchange(ref locked, 0);
        }
    }

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

        public static Locker Lock(ILockable lockable, CancellationToken ctk = default)
        {
            lockable.Lock(ctk);

            return new(lockable);
        }
    }
}
