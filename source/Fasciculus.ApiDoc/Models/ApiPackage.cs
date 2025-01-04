using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiPackage : ApiElement
    {
        public string Description { get; set; } = string.Empty;
        public ApiNamespaces Namespaces { get; }

        public ApiPackage(PackageInfo package)
            : base(package)
        {
            Namespaces = new(package.Namespaces);
        }
    }
}
