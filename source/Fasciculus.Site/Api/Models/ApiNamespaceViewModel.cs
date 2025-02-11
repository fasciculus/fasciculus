using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiNamespaceViewModel : ApiSymbolViewModel<INamespaceSymbol>
    {
        public required string Description { get; init; }

        public required string Content { get; init; }
    }
}
