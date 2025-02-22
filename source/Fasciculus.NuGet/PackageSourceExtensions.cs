using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet
{
    public static class PackageSourceExtensions
    {
        public static SourceRepository GetRepository(this PackageSource packageSource)
        {
            return packageSource.ProtocolVersion switch
            {
                2 => Repository.Factory.GetCoreV2(packageSource),
                _ => Repository.Factory.GetCoreV3(packageSource.Source),
            };
        }

        public static SourceRepositories GetRepositories(this IEnumerable<PackageSource> packageSources)
        {
            IEnumerable<SourceRepository> sourceRepositories = packageSources.Select(x => x.GetRepository());

            return new(sourceRepositories);
        }
    }
}
