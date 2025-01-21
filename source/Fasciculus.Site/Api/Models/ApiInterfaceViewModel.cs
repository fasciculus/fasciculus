using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiInterfaceViewModel : ApiSymbolInfoViewModel
    {
        public required InterfaceSymbol Interface { get; init; }
    }
}
