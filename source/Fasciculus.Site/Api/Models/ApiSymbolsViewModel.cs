using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiSymbolsViewModel<T> : ViewModel
        where T : notnull, ISymbol
    {
        public required T[] Symbols { get; init; }
    }
}
