using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEventViewModel : ApiSymbolInfoViewModel
    {
        public required IEventSymbol Event { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
