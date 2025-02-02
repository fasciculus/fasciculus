using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiSymbolViewModel<T> : ViewModel
        where T : notnull, ISymbol
    {
        public required T Symbol { get; init; }
    }
}
