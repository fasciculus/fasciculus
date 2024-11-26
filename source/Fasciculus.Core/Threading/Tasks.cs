using System;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    public static class Tasks
    {
        public static Task Start(Action action)
            => Task.Factory.StartNew(action);

        public static Task<T> Start<T>(Func<T> func)
            => Task<T>.Factory.StartNew(func);

        public static Task LongRunning(Action action)
            => Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);

        public static Task<T> LongRunning<T>(Func<T> func)
            => Task.Factory.StartNew(func, TaskCreationOptions.LongRunning);
    }
}
