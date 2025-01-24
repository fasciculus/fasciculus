using Fasciculus.Threading;
using Fasciculus.Threading.Synchronization;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Threading;

namespace Fasciculus.NuGet.Services
{
    public class NuGetResources : INuGetResources
    {
        public const string DefaultRepositoryUrl = "https://api.nuget.org/v3/index.json";

        private readonly SourceRepository repository;

        private readonly TaskSafeMutex mutex = new();

        private FindPackageByIdResource? findPackageById;

        public FindPackageByIdResource FindPackageById => GetFindPackageById();

        private PackageMetadataResource? packageMetadata;

        public PackageMetadataResource PackageMetadata => GetPackageMetadata();

        public NuGetResources()
        {
            repository = Repository.Factory.GetCoreV3(DefaultRepositoryUrl);
        }

        private FindPackageByIdResource GetFindPackageById()
        {
            using Locker locker = Locker.Lock(mutex);

            if (findPackageById is null)
            {
                findPackageById = Tasks.Wait(repository.GetResourceAsync<FindPackageByIdResource>(CancellationToken.None));
            }

            return findPackageById;
        }

        private PackageMetadataResource GetPackageMetadata()
        {
            using Locker locker = Locker.Lock(mutex);

            if (packageMetadata is null)
            {
                packageMetadata = Tasks.Wait(repository.GetResourceAsync<PackageMetadataResource>(CancellationToken.None));
            }

            return packageMetadata;
        }
    }
}
