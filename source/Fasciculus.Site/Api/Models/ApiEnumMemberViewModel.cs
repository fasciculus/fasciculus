using Fasciculus.CodeAnalysis.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiEnumMemberViewModel : ApiSymbolInfoViewModel
    {
        public required EnumMemberSymbol Member { get; init; }
    }
}
