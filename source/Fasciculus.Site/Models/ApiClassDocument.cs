using Fasciculus.ApiDoc.Models;

namespace Fasciculus.Site.Models
{
    public class ApiClassDocument : Document
    {
        public required ApiClass Class { get; init; }
    }
}
