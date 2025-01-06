using Fasciculus.ApiDoc.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Models
{
    public class ApiIndexDocument : SiteDocument
    {
        public required IEnumerable<ApiPackage> Packages { get; init; }
    }
}
