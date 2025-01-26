using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEventViewModel : ApiSymbolInfoViewModel
    {
        public required IEventSymbol Event { get; init; }
    }
}
