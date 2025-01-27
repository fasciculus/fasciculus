using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPropertyViewModel : ApiSymbolInfoViewModel
    {
        public required IPropertySymbol Property { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
