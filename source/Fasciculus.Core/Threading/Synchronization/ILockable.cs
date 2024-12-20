using System.Threading;

namespace Fasciculus.Threading.Synchronization
{
    public interface ILockable
    {
        public void Lock(CancellationToken ctk);

        public void Unlock();
    }
}
