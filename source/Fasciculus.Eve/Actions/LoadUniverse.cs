using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fasciculus.Eve.Actions
{
    public static class LoadUniverse
    {
        public static async Task LoadAsync()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            await LoadRegions.LoadAsync();

            Console.WriteLine("LoadUniverse: " + stopwatch.ElapsedMilliseconds);
        }
    }
}
