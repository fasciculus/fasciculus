using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Environment;

namespace Fasciculus.Core.Tests.Threading
{
    [TestClass]
    public class TaskAndThreadTests : TestsBase
    {
        const int TASK_COUNT = 50;
        const int MIN_DELAY = 50;
        const int MAX_DELAY = 100;

        const TaskCreationOptions TASK_OPTIONS
            = TaskCreationOptions.PreferFairness
            | TaskCreationOptions.HideScheduler;

        private readonly IEnumerable<int> range = Enumerable
            .Range(0, TASK_COUNT);

        private readonly ThreadLocal<Random> threadLocalRandom
            = new(() => new Random());

        private long totalDelay = 0;

        private readonly Stopwatch stopwatch = Stopwatch.StartNew();

        private int GetThreadLocalDelay()
            => threadLocalRandom.Value!.Next(MIN_DELAY, MAX_DELAY);

        private static Task<T> CreateTask<T>(Func<T> func)
        {
            return Task.Factory.StartNew(func, TASK_OPTIONS);
        }

        private void Sleep()
        {
            int delay = GetThreadLocalDelay();

            Task.Delay(GetThreadLocalDelay())
                .GetAwaiter()
                .GetResult();

            Interlocked.Add(ref totalDelay, delay);
        }

        private async Task SleepAsync()
        {
            int delay = GetThreadLocalDelay();

            await Task.Delay(delay);

            Interlocked.Add(ref totalDelay, delay);
        }

        private SortedSet<int> Work()
        {
            SortedSet<int> threadIds = [];

            threadIds.Add(CurrentManagedThreadId);
            Sleep();
            threadIds.Add(CurrentManagedThreadId);

            return threadIds;
        }

        private async Task<SortedSet<int>> WorkAsync()
        {
            SortedSet<int> threadIds = [];

            threadIds.Add(CurrentManagedThreadId);
            await SleepAsync();
            threadIds.Add(CurrentManagedThreadId);

            return threadIds;
        }

        [TestMethod]
        public void TestWorker()
        {
            Task<SortedSet<int>>[] tasks = range
                .Select(_ => CreateTask(() => Work()))
                .ToArray();

            Task.WaitAll(tasks);

            SortedSet<int>[] threadIdSets = tasks
                .Select(t => t.Result)
                .ToArray();

            ProcessResults(threadIdSets);
        }

        [TestMethod]
        public void TestAsyncWorker()
        {
            Task<SortedSet<int>>[] tasks = range
                .Select(_ => WorkAsync())
                .ToArray();

            Task.WaitAll(tasks);

            SortedSet<int>[] threadIdSets = tasks
                .Select(t => t.Result)
                .ToArray();

            ProcessResults(threadIdSets);
        }

        [TestMethod]
        public void TestParallel()
        {
            SortedSet<int>[] threadIdSets = range.AsParallel()
                .Select(_ => Work())
                .ToArray();

            ProcessResults(threadIdSets);
        }

        [TestMethod]
        public void TestAsyncParallel()
        {
            SortedSet<int>[] threadIdSets = range
                .AsParallel()
                .Select(async _ => await WorkAsync())
                .Select(t => t.Result)
                .ToArray();

            ProcessResults(threadIdSets);
        }

        private void ProcessResults(SortedSet<int>[] threadIdSets)
        {
            int taskSwitchingTaskCount = range
                .Where(i => threadIdSets[i].Count > 1)
                .Count();

            SortedSet<int> threadIdsUsed
                = new(threadIdSets.SelectMany(s => s));

            Log($"thread switching tasks: {taskSwitchingTaskCount}");
            Log($"threads used          : {threadIdsUsed.Count}");
            Log($"average sleep         : {totalDelay / TASK_COUNT} ms");
            Log($"duration              : {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
