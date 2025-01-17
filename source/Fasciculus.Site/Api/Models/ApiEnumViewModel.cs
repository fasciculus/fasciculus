using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEnumViewModel : ViewModel
    {
        public required EnumSymbol Enum { get; init; }
    }
}
