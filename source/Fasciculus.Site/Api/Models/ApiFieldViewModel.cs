using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiFieldViewModel : ApiSymbolInfoViewModel
    {
        public required FieldSymbol Field { get; init; }
    }
}
