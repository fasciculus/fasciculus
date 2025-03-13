using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet.Protocol
{
    public class NuGetRepositories : IEnumerable<NuGetRepository>
    {
        private readonly NuGetRepository[] repositories;

        public int Count => repositories.Length;

        public NuGetRepositories(IEnumerable<NuGetRepository> repositories)
        {
            this.repositories = [.. repositories];
        }

        public IEnumerator<NuGetRepository> GetEnumerator()
            => repositories.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => repositories.AsEnumerable().GetEnumerator();
    }
}
