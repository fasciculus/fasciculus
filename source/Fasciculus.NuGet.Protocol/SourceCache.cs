using Fasciculus.Threading;
using NuGet.Protocol.Core.Types;

namespace Fasciculus.NuGet.Protocol
{
    public static class SourceCache
    {
        public static AsyncLazy<SourceCacheContext> Default { get; }
            = new AsyncLazy<SourceCacheContext>(() => new());
    }
}
