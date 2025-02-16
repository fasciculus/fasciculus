using Fasciculus.Site.Models;

namespace Fasciculus.Site.Specifications.Models
{
    public class SpecificationPackageViewModel : ViewModel
    {
        public required SpecificationEntry[] Entries { get; init; }
    }
}
