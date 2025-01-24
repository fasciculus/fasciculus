using Fasciculus.Threading;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Services
{
    public class NuGetResources : INuGetResources
    {
        public const string DefaultRepositoryUrl = "https://api.nuget.org/v3/index.json";

        private readonly SourceRepository repository;

        public NuGetResources()
        {
            repository = Repository.Factory.GetCoreV3(DefaultRepositoryUrl);
        }

        public async Task<FindPackageByIdResource> GetFindPackageByIdResourceAsync(CancellationToken? ctk = null)
        {
            return await repository.GetResourceAsync<FindPackageByIdResource>(ctk.OrNone());
        }

        public FindPackageByIdResource GetFindPackageByIdResource(CancellationToken? ctk = null)
        {
            return Tasks.Wait(GetFindPackageByIdResourceAsync(ctk));
        }
    }
}
