using Fasciculus.ApiDoc.Models;
using System.Linq;

namespace Fasciculus.Site.Models
{
    public class ApiNamespaceDocument : SiteDocument
    {
        public required ApiNamespace Namespace { get; init; }

        public required ApiClasses Classes { get; init; }
        public bool HasClasses => Classes.Any();
    }
}
