using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Models
{
    public class ApiPackageDocument : SiteDocument
    {
        public required PackageSymbol Package { get; init; }
    }
}
