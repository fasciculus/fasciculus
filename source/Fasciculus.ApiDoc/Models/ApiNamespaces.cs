using Fasciculus.CodeAnalysis.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiNamespaces : IEnumerable<ApiNamespace>
    {
        private readonly List<ApiNamespace> namespaces;

        public ApiNamespaces(NamespaceCollection namespaces, ApiPackage package)
        {
            this.namespaces = namespaces.Select(n => new ApiNamespace(n, package)).ToList();
        }

        public IEnumerator<ApiNamespace> GetEnumerator()
            => namespaces.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => namespaces.GetEnumerator();
    }
}
