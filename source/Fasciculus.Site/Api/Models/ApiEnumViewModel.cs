using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEnumViewModel : ApiSymbolInfoViewModel
    {
        public required EnumSymbol Enum { get; init; }
    }
}
