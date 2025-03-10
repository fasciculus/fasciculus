using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.Threading.Experiment
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Task.Yield();

            string current = Task.CurrentId?.ToString() ?? "null";
            Task<Tuple<int, int>>[] tasks = [.. Enumerable.Range(0, 2).Select(Work1)];

            Task.WaitAll(tasks);

            foreach (Task<Tuple<int, int>> task in tasks)
            {
                Tuple<int, int> result = task.Result;

                Log($"{result.Item1} {result.Item1}");
            }

            Log($"main: {current}");
        }

        private static async Task<Tuple<int, int>> Work1(int _)
        {
            int id = Task.CurrentId ?? 0;
            int child = await Work2(0);

            return Tuple.Create(id, child);
        }

        private static async Task<int> Work2(int _)
        {
            int id = Task.CurrentId ?? 0;

            await Task.Yield();

            return id;
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
