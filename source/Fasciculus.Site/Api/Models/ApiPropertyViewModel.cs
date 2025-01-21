using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPropertyViewModel : ApiSymbolInfoViewModel
    {
        public required PropertySymbol Property { get; init; }
    }
}
