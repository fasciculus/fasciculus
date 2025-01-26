using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System.Collections.Generic;

namespace Fasciculus.Site.Api.Models
{
    public class ApiConstructorsViewModel : ViewModel
    {
        public required IEnumerable<IConstructorSymbol> Constructors { get; init; }
    }
}
