using NuGet.Protocol.Core.Types;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Services
{
    public interface INuGetResources
    {
        public Task<FindPackageByIdResource> GetFindPackageByIdResourceAsync(CancellationToken? ctk = null);

        public FindPackageByIdResource GetFindPackageByIdResource(CancellationToken? ctk = null);
    }
}
