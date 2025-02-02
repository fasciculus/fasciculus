using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Api.Models
{
    public class ApiNamespaceViewModel : ViewModel
    {
        public required INamespaceSymbol Namespace { get; init; }

        public IEnumerable<IEnumSymbol> Enums => Namespace.Enums;
        public bool HasEnums => Enums.Any();

        public IEnumerable<IInterfaceSymbol> Interfaces => Namespace.Interfaces;
        public bool HasInterfaces => Interfaces.Any();

        public IEnumerable<IClassSymbol> Classes => Namespace.Classes;
        public bool HasClasses => Classes.Any();

    }
}
