using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiNamespace : ApiElement
    {
        public override UriPath Link { get; }
        public ApiClasses Classes { get; }

        private readonly SortedSet<string> packages;
        public IEnumerable<string> Packages => packages;

        public string Description { get; set; } = string.Empty;

        public ApiNamespace(NamespaceInfo @namespace, ApiPackage package)
            : base(@namespace)
        {
            Link = package.Link.Append(Name);
            Classes = new(@namespace.Classes, this);

            packages = new(@namespace.Packages);
        }
    }
}
