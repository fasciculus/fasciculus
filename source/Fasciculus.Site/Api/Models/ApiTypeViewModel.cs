using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System;

namespace Fasciculus.Site.Api.Models
{
    public class ApiTypeViewModel : ViewModel
    {
        public required Symbol Symbol { get; init; }

        public required Uri[] SourceUris { get; init; }
    }
}
