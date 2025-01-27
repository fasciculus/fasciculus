using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEnumViewModel : ApiSymbolInfoViewModel
    {
        public required IEnumSymbol Enum { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
