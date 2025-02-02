using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEventViewModel : ViewModel
    {
        public required IEventSymbol Event { get; init; }
    }
}
