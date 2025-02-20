using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Fasciculus.NuGet.Services
{
    public interface INuGetResources
    {
        public FindPackageByIdResource FindPackageById { get; }

        public FindLocalPackagesResource FindLocalPackages { get; }

        public PackageMetadataResource PackageMetadata { get; }
    }
}
