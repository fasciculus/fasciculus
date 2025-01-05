using Fasciculus.ApiDoc.Models;
using System.Linq;

namespace Fasciculus.GitHub.Models
{
    public class ApiNamespaceDocument : Document
    {
        public required ApiNamespace Namespace { get; init; }

        public required ApiClasses Classes { get; init; }
        public bool HasClasses => Classes.Any();
    }
}
