using System.Threading;

namespace Fasciculus.Threading.Synchronization
{
    public class TaskSafeMutex : ILockable
    {
        private long locked = 0;

        public void Lock(CancellationToken ctk)
        {
            while (true)
            {
                ctk.ThrowIfCancellationRequested();

                if (Interlocked.CompareExchange(ref locked, 1, 0) == 0)
                {
                    return;
                }

                Tasks.Sleep(0);
            }
        }

        public void Unlock()
        {
            Interlocked.Exchange(ref locked, 0);
        }
    }
}
