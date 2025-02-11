using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPackageViewModel : ApiSymbolViewModel<IPackageSymbol>
    {
        public required string Description { get; init; }

        public required string Content { get; init; }
    }
}
