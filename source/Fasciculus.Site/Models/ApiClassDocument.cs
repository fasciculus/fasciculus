using Fasciculus.ApiDoc.Models;

namespace Fasciculus.GitHub.Models
{
    public class ApiClassDocument : Document
    {
        public required ApiClass Class { get; init; }
    }
}
