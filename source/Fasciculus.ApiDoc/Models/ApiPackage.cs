using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiPackage : ApiElement
    {
        public override ApiLink Link { get; }
        public ApiNamespaces Namespaces { get; }

        public string Description { get; set; } = string.Empty;

        public ApiPackage(PackageInfo package)
            : base(package)
        {
            Link = new ApiLink(Name);
            Namespaces = new(package.Namespaces, this);
        }
    }
}
