using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.ApiDoc.Models
{
    public class ApiNamespace : ApiElement
    {
        public string Description { get; set; } = string.Empty;

        public ApiNamespace(NamespaceInfo @namespace)
            : base(@namespace)
        {

        }
    }
}
