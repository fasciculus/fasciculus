using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPackageDocument : SiteDocument
    {
        public required PackageSymbol Package { get; init; }
    }
}
