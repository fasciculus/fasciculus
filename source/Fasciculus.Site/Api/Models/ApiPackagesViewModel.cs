using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPackagesViewModel : ViewModel
    {
        public required IPackageSymbol Combined { get; init; }

        public required IEnumerable<IPackageSymbol> Packages { get; init; }
    }
}
