using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiClassViewModel : ApiSymbolInfoViewModel
    {
        public required IClassSymbol Class { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
