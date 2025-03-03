using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.Threading.Experiment
{
    internal class Program
    {
        private readonly TaskScheduler defaultScheduler;
        private readonly TaskScheduler customScheduler;

        private readonly CancellationToken ctk = CancellationToken.None;

        private Program()
        {
            defaultScheduler = TaskScheduler.Default;
            customScheduler = new CustomTaskScheduler(defaultScheduler);
        }

        private void Run()
        {
            RunWork();
            RunWorkParallel();
            RunWorkAsync();
        }

        private void RunWork()
        {
            Logger.Log("--------- RunWork ---------");

            Stopwatch sw = Stopwatch.StartNew();
            Task<SortedSet<int>>[] tasks = [.. Enumerable.Range(0, 20).Select(CreateWorkTask)];

            Task.WaitAll(tasks);
            Logger.Log($"ElapsedMilliseconds = {sw.ElapsedMilliseconds}");

            SortedSet<int>[] results = [.. tasks.Select(x => x.Result)];
            int singles = results.Count(x => x.Count == 1);
            int multis = results.Count(x => x.Count > 1);

            Logger.Log($"singles = {singles}, multis = {multis}");

            int[] threads = [.. results.SelectMany(x => x).Distinct()];

            Logger.Log($"threads = {string.Join(", ", threads)}");
        }

        private void RunWorkParallel()
        {
            Logger.Log("--------- RunWorkParallel ---------");

            Stopwatch sw = Stopwatch.StartNew();
            SortedSet<int>[] results = [.. Enumerable.Range(0, 20).AsParallel().Select(_ => Work())];

            Logger.Log($"ElapsedMilliseconds = {sw.ElapsedMilliseconds}");

            int singles = results.Count(x => x.Count == 1);
            int multis = results.Count(x => x.Count > 1);

            Logger.Log($"singles = {singles}, multis = {multis}");

            int[] threads = [.. results.SelectMany(x => x).Distinct()];

            Logger.Log($"threads = {string.Join(", ", threads)}");
        }

        private void RunWorkAsync()
        {
            Logger.Log("--------- RunWorkAsync ---------");

            Stopwatch sw = Stopwatch.StartNew();
            Task<SortedSet<int>>[] tasks = [.. Enumerable.Range(0, 20).Select(CreateWorkAsyncTask)];

            Task.WaitAll(tasks);
            Logger.Log($"ElapsedMilliseconds = {sw.ElapsedMilliseconds}");

            SortedSet<int>[] results = [.. tasks.Select(x => x.Result)];
            int singles = results.Count(x => x.Count == 1);
            int multis = results.Count(x => x.Count > 1);

            Logger.Log($"singles = {singles}, multis = {multis}");

            int[] threads = [.. results.SelectMany(x => x).Distinct()];

            Logger.Log($"threads = {string.Join(", ", threads)}");
        }

        private Task<SortedSet<int>> CreateWorkTask(int _)
        {
            //return Task.Factory.StartNew(Work, ctk, TaskCreationOptions.PreferFairness, customScheduler);
            return Task.Factory.StartNew(Work, ctk, TaskCreationOptions.PreferFairness, defaultScheduler);
        }

        private Task<SortedSet<int>> CreateWorkAsyncTask(int _)
        {
            return WorkAsync();
        }

        private static SortedSet<int> Work()
        {
            SortedSet<int> result = [];

            result.Add(Environment.CurrentManagedThreadId);

            for (int i = 0; i < 5; ++i)
            {
                Task.Delay(10).WaitFor();
                result.Add(Environment.CurrentManagedThreadId);
            }

            return result;
        }

        private static async Task<SortedSet<int>> WorkAsync()
        {
            SortedSet<int> result = [];

            result.Add(Environment.CurrentManagedThreadId);

            for (int i = 0; i < 5; ++i)
            {
                await Task.Delay(10);
                result.Add(Environment.CurrentManagedThreadId);
            }

            return result;
        }

        static void Main(string[] args)
        {
            Program program = new();

            program.Run();
        }
    }
}
