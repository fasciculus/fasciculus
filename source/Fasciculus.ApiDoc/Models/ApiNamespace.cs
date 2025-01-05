using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiNamespace : ApiElement
    {
        public string Description { get; set; } = string.Empty;

        private readonly SortedSet<string> packages;
        public IEnumerable<string> Packages => packages;

        public override string Link { get; }

        public ApiNamespace(NamespaceInfo @namespace, ApiPackage package)
            : base(@namespace)
        {
            packages = new(@namespace.Packages);

            Link = $"{package.Link}/{Name}";
        }
    }
}
