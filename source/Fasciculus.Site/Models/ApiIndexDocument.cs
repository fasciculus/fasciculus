using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Models
{
    public class ApiIndexDocument : SiteDocument
    {
        public required IEnumerable<PackageSymbol> Packages { get; init; }
    }
}
