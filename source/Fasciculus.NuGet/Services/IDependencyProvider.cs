using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;

namespace Fasciculus.NuGet.Services
{
    public interface IDependencyProvider
    {
        public IPackageSearchMetadata[] GetDependencies(IEnumerable<PackageIdentity> packages, NuGetFramework targetFramework);
    }
}
