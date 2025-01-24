using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using System.Threading;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Services
{
    public interface IMetadataProvider
    {
        public Task<IPackageSearchMetadata> GetMetadataAsync(PackageIdentity package, CancellationToken? ctk = null);

        public IPackageSearchMetadata? GetMetadata(PackageIdentity package, CancellationToken? ctk = null);
    }
}
