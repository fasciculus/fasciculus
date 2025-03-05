using Fasciculus.Threading;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Maui.Threading
{
    public static class Threads
    {
        public static void RunInMainThread(Action action)
            => Tasks.Wait(RunInMainThreadAync(action));

        public static Task RunInMainThreadAync(Action action)
            => MainThread.InvokeOnMainThreadAsync(action);
    }
}
