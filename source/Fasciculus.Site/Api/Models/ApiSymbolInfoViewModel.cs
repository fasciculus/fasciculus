using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System;

namespace Fasciculus.Site.Api.Models
{
    public class ApiSymbolInfoViewModel : ViewModel
    {
        public required ISymbol Symbol { get; init; }

        public required Uri[] SourceUris { get; init; }
    }
}
