using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;

namespace Fasciculus.Site.Api.Models
{
    public class ApiConstructorsViewModel : ViewModel
    {
        public required ConstructorList Constructors { get; init; }
    }
}
