using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPackageViewModel : ApiViewModel
    {
        public required PackageSymbol Package { get; init; }
    }
}
