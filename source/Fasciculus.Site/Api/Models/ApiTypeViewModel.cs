using Fasciculus.Site.Models;
using System;

namespace Fasciculus.Site.Api.Models
{
    public class ApiTypeViewModel : ViewModel
    {
        public required Uri[] SourceUris { get; init; }
    }
}
