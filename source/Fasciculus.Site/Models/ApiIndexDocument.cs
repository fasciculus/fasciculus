using Fasciculus.ApiDoc.Models;
using System.Collections.Generic;

namespace Fasciculus.GitHub.Models
{
    public class ApiIndexDocument : Document
    {
        public required IEnumerable<ApiPackage> Packages { get; init; }
    }
}
