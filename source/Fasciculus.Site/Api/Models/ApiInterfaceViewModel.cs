using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiInterfaceViewModel : ApiSymbolInfoViewModel
    {
        public required IInterfaceSymbol Interface { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
