using System;
using System.Diagnostics;

namespace Fasciculus.Threading.Experiment
{
    internal class Logger
    {
        private static readonly object lockObject = new();

        public static void Log(string message)
        {
            lock (lockObject)
            {
                Console.WriteLine(message);
                Debug.WriteLine(message);
            }
        }
    }
}
