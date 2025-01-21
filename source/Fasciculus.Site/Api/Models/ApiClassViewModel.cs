using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiClassViewModel : ApiSymbolInfoViewModel
    {
        public required ClassSymbol Class { get; init; }
    }
}
