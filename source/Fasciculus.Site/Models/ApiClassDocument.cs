using Fasciculus.ApiDoc.Models;

namespace Fasciculus.Site.Models
{
    public class ApiClassDocument : SiteDocument
    {
        public required ApiClass Class { get; init; }
    }
}
