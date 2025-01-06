using Fasciculus.ApiDoc.Models;

namespace Fasciculus.Site.Models
{
    public class ApiPackageDocument : SiteDocument
    {
        public required ApiPackage Package { get; init; }
    }
}
