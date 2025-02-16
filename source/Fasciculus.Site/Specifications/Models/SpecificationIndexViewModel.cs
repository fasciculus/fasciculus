using Fasciculus.Site.Models;

namespace Fasciculus.Site.Specifications.Models
{
    public class SpecificationIndexViewModel : ViewModel
    {
        public required SpecificationPackage[] Packages { get; init; }
    }
}
