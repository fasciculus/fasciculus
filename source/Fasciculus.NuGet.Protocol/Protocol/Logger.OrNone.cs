using NuGet.Common;

namespace Fasciculus.NuGet.Protocol
{
    public static class NuGetLoggerExtensions
    {
        public static ILogger OrNone(this ILogger? logger)
            => logger is null ? NullLogger.Instance : logger;
    }
}