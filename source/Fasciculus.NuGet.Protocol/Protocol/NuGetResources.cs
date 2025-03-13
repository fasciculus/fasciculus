using Fasciculus.Threading;
using NuGet.Protocol.Core.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Protocol
{
    public class NuGetResources
    {
        private readonly SourceRepository[] repositories;

        public AsyncLazy<FindPackageByIdResource[]> FindPackageById { get; }

        public NuGetResources(IEnumerable<SourceRepository> repositories)
        {
            this.repositories = [.. repositories];

            FindPackageById = new(GetFindPackageById);
        }

        private async Task<FindPackageByIdResource[]> GetFindPackageById()
            => await GetResources<FindPackageByIdResource>();

        private async Task<T[]> GetResources<T>()
            where T : class, INuGetResource
        {
            IEnumerable<T> result = [];

            foreach (SourceRepository repository in this.repositories)
            {
                T? resource = await repository.GetResourceAsync<T>();

                if (resource is not null)
                {
                    result = result.Append(resource);
                }
            }

            return [.. result];
        }
    }
}
