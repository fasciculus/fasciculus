using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Models
{
    public class ApiClassDocument : SiteDocument
    {
        public required ClassSymbol Class { get; init; }
    }
}
