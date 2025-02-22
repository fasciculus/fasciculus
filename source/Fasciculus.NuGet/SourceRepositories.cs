using NuGet.Protocol.Core.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.NuGet
{
    public class SourceRepositories : IEnumerable<SourceRepository>
    {
        private readonly SourceRepository[] sourceRepositories;

        public int Count => sourceRepositories.Length;

        public SourceRepositories(IEnumerable<SourceRepository> sourceRepositories)
        {
            this.sourceRepositories = [.. sourceRepositories];
        }

        public IEnumerator<SourceRepository> GetEnumerator()
            => sourceRepositories.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => sourceRepositories.AsEnumerable().GetEnumerator();
    }
}
