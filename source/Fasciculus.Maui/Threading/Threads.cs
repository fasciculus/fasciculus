using Fasciculus.Threading;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Threading.Tasks;

namespace Fasciculus.Maui.Threading
{
    public static class Threads
    {
        public static void RunInMainThread(Action action)
            => RunInMainThreadAync(action).WaitFor();

        public static Task RunInMainThreadAync(Action action)
            => MainThread.InvokeOnMainThreadAsync(action);
    }
}
