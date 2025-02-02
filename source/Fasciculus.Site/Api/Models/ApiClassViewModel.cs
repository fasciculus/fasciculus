using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiClassViewModel : ViewModel
    {
        public required IClassSymbol Class { get; init; }
    }
}
