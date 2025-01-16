using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPackagesViewModel : ViewModel
    {
        public required PackageSymbol Combined { get; init; }

        public required IEnumerable<PackageSymbol> Packages { get; init; }
    }
}
