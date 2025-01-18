using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEnumViewModel : ApiTypeViewModel
    {
        public required EnumSymbol Enum { get; init; }
    }
}
