using Fasciculus.CodeAnalysis.Models;
using Fasciculus.Net.Navigating;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class TypeReceiver : ITypeReceiver
    {
        public UriPath Link { get; }

        protected readonly List<ClassSymbol> classes = [];
        protected readonly List<EnumSymbol> enums = [];

        public TypeReceiver(UriPath link)
        {
            Link = link;
        }

        public void Add(ClassSymbol @class)
        {
            classes.Add(@class);
        }

        public void Add(EnumSymbol @enum)
        {
            enums.Add(@enum);
        }
    }
}
