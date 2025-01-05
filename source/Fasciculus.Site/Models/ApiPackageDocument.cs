using Fasciculus.ApiDoc.Models;

namespace Fasciculus.Site.Models
{
    public class ApiPackageDocument : Document
    {
        public required ApiPackage Package { get; init; }
    }
}
