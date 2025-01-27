using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiMemberViewModel : ApiSymbolInfoViewModel
    {
        public required IMemberSymbol Member { get; init; }

        public required ApiAppliesTo AppliesTo { get; init; }
    }
}
