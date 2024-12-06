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

        public static Task[] WaitAll(Task[] tasks)
        {
            Task.WaitAll(tasks);

            return tasks;
        }

        public static Task<T>[] WaitAll<T>(Task<T>[] tasks)
        {
            Task.WaitAll(tasks);

            return tasks;
        }

        public static void Sleep(int milliseconds)
            => Tasks.Wait(Task.Delay(milliseconds));

        public static void Wait(Task task)
            => task.GetAwaiter().GetResult();

        public static T Wait<T>(Task<T> task)
            => task.GetAwaiter().GetResult();

        public static Tuple<T1, T2> Wait<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            Task.WaitAll([task1, task2]);

            return new(task1.Result, task2.Result);
        }

        public static Tuple<T1, T2, T3> Wait<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
        {
            Task.WaitAll([task1, task2, task3]);

            return new(task1.Result, task2.Result, task3.Result);
        }

        public static Tuple<T1, T2, T3, T4> Wait<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4)
        {
            Task.WaitAll([task1, task2, task3, task4]);

            return new(task1.Result, task2.Result, task3.Result, task4.Result);
        }
    }
}
