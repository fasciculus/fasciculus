using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet.Protocol
{
    public class NuGetRepositories : IEnumerable<NuGetRepository>
    {
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
