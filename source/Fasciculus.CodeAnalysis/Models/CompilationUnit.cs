using System.Collections.Generic;

namespace Fasciculus.CodeAnalysis.Models
{
    public class CompilationUnit
    {
        private readonly NamespaceList namespaces;

        public IEnumerable<NamespaceSymbol> Namespaces => namespaces;

        public CompilationUnit(NamespaceList namespaces)
        {
            this.namespaces = namespaces;
        }
    }
}
