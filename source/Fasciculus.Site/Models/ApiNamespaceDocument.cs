using Fasciculus.ApiDoc.Models;

namespace Fasciculus.GitHub.Models
{
    public class ApiNamespaceDocument : Document
    {
        public required ApiNamespace Namespace { get; init; }
    }
}
