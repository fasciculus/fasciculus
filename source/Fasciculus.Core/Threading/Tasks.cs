using System;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static void Run(this Task task)
            => task.GetAwaiter().GetResult();

        public static T Run<T>(this Task<T> task)
            => task.GetAwaiter().GetResult();
    }
}

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
