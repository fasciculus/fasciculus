using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEventViewModel : ApiSymbolInfoViewModel
    {
        public required EventSymbol Event { get; init; }
    }
}
