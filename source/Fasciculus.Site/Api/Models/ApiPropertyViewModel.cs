using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPropertyViewModel : ViewModel
    {
        public required PropertySymbol Property { get; init; }
    }
}
