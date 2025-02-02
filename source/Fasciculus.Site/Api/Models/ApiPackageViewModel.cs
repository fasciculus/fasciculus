using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPackageViewModel : ViewModel
    {
        public required IPackageSymbol Package { get; init; }

        public required Uri PackageUri { get; init; }
    }
}
