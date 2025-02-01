using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPropertyViewModel : ViewModel
    {
        public required IPropertySymbol Property { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
