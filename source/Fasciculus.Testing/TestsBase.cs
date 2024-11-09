
#if DEBUG
using System.Diagnostics;
#else
using System;
#endif

namespace Fasciculus
{
    public class TestsBase
    {
        protected void Log(string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#else
            Console.WriteLine(message);
#endif
        }
    }
}
