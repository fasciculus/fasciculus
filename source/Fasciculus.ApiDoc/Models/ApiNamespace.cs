using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiNamespace : ApiElement
    {
        public override ApiLink Link { get; }
        public ApiClasses Classes { get; }

        private readonly SortedSet<string> packages;
        public IEnumerable<string> Packages => packages;

        public string Description { get; set; } = string.Empty;

        public ApiNamespace(NamespaceInfo @namespace, ApiPackage package)
            : base(@namespace)
        {
            Link = package.Link.Combine(Name);
            Classes = new(@namespace.Classes, this);

            packages = new(@namespace.Packages);
        }
    }
}
