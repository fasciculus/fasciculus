﻿using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading.Synchronization
{
    public class ReentrantTaskSafeMutex : ILockable
    {
        private readonly TaskSafeMutex mutex = new();

        private int? owner = null;
        private int depth = 0;

        public void Lock(CancellationToken ctk)
        {
            while (true)
            {
                using Locker locker = Locker.Lock(mutex, ctk);

                ctk.ThrowIfCancellationRequested();

                int newOwner = Task.CurrentId ?? 0;

                if (owner is null)
                {
                    owner = newOwner;
                    depth = 1;
                    return;
                }

                if (owner == newOwner)
                {
                    ++depth;
                    return;
                }

                Tasks.Sleep(0);
            }
        }

        public void Unlock()
        {
            using Locker locker = Locker.Lock(mutex);

            --depth;

            if (depth == 0)
            {
                owner = null;
            }
        }
    }
}