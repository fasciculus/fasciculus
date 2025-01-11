using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPackageDocument : ViewModel
    {
        public required PackageSymbol Package { get; init; }
    }
}
