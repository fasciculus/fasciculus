using System;
using System.Threading.Tasks;

namespace Fasciculus.Threading
{
    public static class Tasks
    {
        public static Task LongRunning(Action action)
        {
            return Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
        }

        public static Task<T> LongRunning<T>(Func<T> func)
        {
            return Task.Factory.StartNew(func, TaskCreationOptions.LongRunning);
        }
    }
}
