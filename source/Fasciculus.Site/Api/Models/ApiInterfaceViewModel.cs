using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiInterfaceViewModel : ViewModel
    {
        public required IInterfaceSymbol Interface { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
