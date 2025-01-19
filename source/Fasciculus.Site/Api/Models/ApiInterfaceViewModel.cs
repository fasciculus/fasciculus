using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiInterfaceViewModel : ApiTypeViewModel
    {
        public required InterfaceSymbol Interface { get; init; }
    }
}
