using Fasciculus.Support;
using System;
using System.Threading;

namespace Fasciculus.Threading
{
    public static class Threads
    {
        private static int? mainThreadId;
        private static SynchronizationContext? mainThreadContext;

        public static void SetMainThread()
        {
            mainThreadId = Environment.CurrentManagedThreadId;
            mainThreadContext = Cond.NotNull(SynchronizationContext.Current);
        }

        public static bool IsInMainThread
            => mainThreadId is not null && mainThreadId == Environment.CurrentManagedThreadId;

        public static void RunInMainThread(Action action)
        {
            if (IsInMainThread)
            {
                action();
            }
            else
            {
                Cond.NotNull(mainThreadContext).Post(_ => action(), null);
            }
        }
    }
}
