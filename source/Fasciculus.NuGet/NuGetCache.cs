using NuGet.Protocol.Core.Types;
using System;

namespace Fasciculus.NuGet
{
    public static class NuGetCache
    {
        private static readonly Lazy<SourceCacheContext> cache = new(() => new(), true);

        public static SourceCacheContext Default => cache.Value;
    }
}
