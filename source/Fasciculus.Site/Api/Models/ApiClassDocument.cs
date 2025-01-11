using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiClassDocument : ViewModel
    {
        public required ClassSymbol Class { get; init; }
    }
}
