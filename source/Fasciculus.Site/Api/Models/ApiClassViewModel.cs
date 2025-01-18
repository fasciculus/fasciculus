using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiClassViewModel : ApiTypeViewModel
    {
        public required ClassSymbol Class { get; init; }
    }
}
