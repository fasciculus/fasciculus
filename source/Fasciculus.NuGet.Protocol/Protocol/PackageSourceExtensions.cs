using Fasciculus.NuGet.Configuration;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet.Protocol
{
    public static class PackageSourceExtensions
    {
        public static NuGetRepository GetRepository(this PackageSource source)
        {
            return source.ProtocolVersion switch
            {
                2 => new(Repository.Factory.GetCoreV2(source)),
                _ => new(Repository.Factory.GetCoreV3(source.Source)),
            };
        }

        public static NuGetRepository GetRepository(this NuGetSource source)
            => source.Source.GetRepository();

        public static NuGetRepositories GetRepositories(this IEnumerable<PackageSource> sources)
            => new(sources.Select(GetRepository));

        public static NuGetRepositories GetRepositories(this IEnumerable<NuGetSource> sources)
            => new(sources.Select(GetRepository));
    }
}
