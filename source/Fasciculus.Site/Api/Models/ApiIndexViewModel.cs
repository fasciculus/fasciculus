using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Api.Models
{
    public class ApiIndexViewModel : ViewModel
    {
        public required IEnumerable<PackageSymbol> Packages { get; init; }
    }
}
