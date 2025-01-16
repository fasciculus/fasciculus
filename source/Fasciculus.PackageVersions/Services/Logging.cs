using System;
using System.Diagnostics;

namespace Fasciculus.PackageVersions.Services
{
    public static class Logging
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
