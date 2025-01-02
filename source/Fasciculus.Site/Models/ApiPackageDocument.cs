using Fasciculus.ApiDoc.Models;

namespace Fasciculus.GitHub.Models
{
    public class ApiPackageDocument : Document
    {
        public required ApiPackage Package { get; init; }
    }
}
