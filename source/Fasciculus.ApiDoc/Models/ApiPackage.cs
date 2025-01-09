using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiPackage : ApiElement
    {
        public override UriPath Link { get; }
        public ApiNamespaces Namespaces { get; }

        public string Description { get; set; } = string.Empty;

        public ApiPackage(PackageInfo package)
            : base(package)
        {
            Link = new(Name);
            Namespaces = new(package.Namespaces, this);
        }
    }
}
