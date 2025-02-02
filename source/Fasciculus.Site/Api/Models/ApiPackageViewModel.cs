using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiPackageViewModel : ViewModel
    {
        public required IPackageSymbol Package { get; init; }
    }
}
