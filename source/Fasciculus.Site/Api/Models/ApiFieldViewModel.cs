using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiFieldViewModel : ApiSymbolInfoViewModel
    {
        public required IFieldSymbol Field { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
