using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiMemberViewModel : ViewModel
    {
        public required IMemberSymbol Member { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
