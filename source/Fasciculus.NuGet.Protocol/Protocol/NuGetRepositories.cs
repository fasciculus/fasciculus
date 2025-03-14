using Fasciculus.NuGet.Configuration;
using Fasciculus.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fasciculus.NuGet.Protocol
{
    public class NuGetRepositories : IEnumerable<NuGetRepository>
    {
        public static AsyncLazy<NuGetRepositories> Remotes { get; } = new(GetRemotes);

        private static async Task<NuGetRepositories> GetRemotes()
        {
            NuGetSources sources = await NuGetSources.Remotes;

            return sources.GetRepositories();
        }

        private readonly NuGetRepository[] repositories;

        public int Count => repositories.Length;

        public NuGetResources Resources { get; }

        public NuGetRepositories(IEnumerable<NuGetRepository> repositories)
        {
            this.repositories = [.. repositories];

            Resources = new(repositories.Select(x => x.Repository));
        }

        public IEnumerator<NuGetRepository> GetEnumerator()
            => repositories.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => repositories.AsEnumerable().GetEnumerator();
    }
}
