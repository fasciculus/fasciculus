using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiFieldViewModel : ViewModel
    {
        public required IFieldSymbol Field { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
