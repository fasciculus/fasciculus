using Fasciculus.Threading;
using NuGet.Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Logging
{
    public class ConsoleLogger : LoggerBase
    {
        public ConsoleLogger(LogLevel level = LogLevel.Information)
            : base(level) { }

        public override void Log(ILogMessage message)
        {
            Console.WriteLine(message.Message);
            Debug.WriteLine(message.Message);
        }

        public override Task LogAsync(ILogMessage message)
        {
            return Tasks.Start(() => { Log(message); });
        }
    }
}
