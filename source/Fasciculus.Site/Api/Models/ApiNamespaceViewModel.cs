using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Site.Models;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.Site.Api.Models
{
    public class ApiNamespaceViewModel : ViewModel
    {
        public required NamespaceSymbol Namespace { get; init; }

        public IEnumerable<EnumSymbol> Enums => Namespace.Enums;
        public bool HasEnums => Enums.Any();

        public IEnumerable<InterfaceSymbol> Interfaces => Namespace.Interfaces;
        public bool HasInterfaces => Interfaces.Any();

        public IEnumerable<ClassSymbol> Classes => Namespace.Classes;
        public bool HasClasses => Classes.Any();

    }
}
