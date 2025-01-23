using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiMemberViewModel : ApiSymbolInfoViewModel
    {
        public required MemberSymbol Member { get; init; }
    }
}
