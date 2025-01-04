using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiNamespace : ApiElement
    {
        public string Description { get; set; } = string.Empty;

        public ApiPackage Package { get; }
        public override string Link { get; }

        public ApiNamespace(NamespaceInfo @namespace, ApiPackage package)
            : base(@namespace)
        {
            Package = package;
            Link = $"{Package.Link}/{Name}";
        }
    }
}
