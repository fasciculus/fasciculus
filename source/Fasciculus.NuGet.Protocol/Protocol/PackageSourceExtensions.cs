using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet.Protocol
{
    public static class PackageSourceExtensions
    {
        public static NuGetRepository GetRepository(this PackageSource packageSource)
        {
            return packageSource.ProtocolVersion switch
            {
                2 => new(Repository.Factory.GetCoreV2(packageSource)),
                _ => new(Repository.Factory.GetCoreV3(packageSource.Source)),
            };
        }

        public static NuGetRepositories GetRepositories(this IEnumerable<PackageSource> packageSources)
            => new(packageSources.Select(GetRepository));
    }
}
