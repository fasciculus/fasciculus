using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiNamespace
    {
        public required INamespaceSymbol Symbol { get; init; }

        public required string Description { get; init; }

        public required string Content { get; init; }
    }
}
