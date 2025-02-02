using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiNamespaceViewModel : ViewModel
    {
        public required INamespaceSymbol Namespace { get; init; }
    }
}
