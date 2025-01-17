using Fasciculus.CodeAnalysis.Models;
using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Compilers.Builders
{
    public class TypeReceiver : ITypeReceiver
    {
        protected readonly List<ClassSymbol> classes = [];

        public void Add(ClassSymbol @class)
        {
            classes.Add(@class);
        }
    }
}
